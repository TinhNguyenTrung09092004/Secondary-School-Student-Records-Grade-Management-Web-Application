using Microsoft.EntityFrameworkCore;
using API.Data;
using API.DTOs;
using API.Models;

namespace API.Services;

public class ClassService : IClassService
{
    private readonly ApplicationDbContext _context;

    public ClassService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ClassDto>> GetAllClassesAsync()
    {
        var classes = await _context.Classes
            .Include(c => c.GradeLevel)
            .Include(c => c.SchoolYear)
            .Include(c => c.Teacher)
            .Include(c => c.ClassAssignments)
            .OrderBy(c => c.ClassName)
            .Select(c => new ClassDto
            {
                ClassId = c.ClassId,
                ClassName = c.ClassName,
                GradeLevelId = c.GradeLevelId,
                GradeLevelName = c.GradeLevel.GradeLevelName,
                SchoolYearId = c.SchoolYearId,
                SchoolYearName = c.SchoolYear.SchoolYearName,
                ClassSize = c.ClassSize,
                CurrentStudentCount = c.ClassAssignments.Count,
                TeacherId = c.TeacherId,
                TeacherName = c.Teacher != null ? c.Teacher.TeacherName : null
            })
            .ToListAsync();

        return classes;
    }

    public async Task<ClassDto?> GetClassByIdAsync(string classId)
    {
        var classEntity = await _context.Classes
            .Include(c => c.GradeLevel)
            .Include(c => c.SchoolYear)
            .Include(c => c.Teacher)
            .Include(c => c.ClassAssignments)
            .FirstOrDefaultAsync(c => c.ClassId == classId);

        if (classEntity == null)
            return null;

        return new ClassDto
        {
            ClassId = classEntity.ClassId,
            ClassName = classEntity.ClassName,
            GradeLevelId = classEntity.GradeLevelId,
            GradeLevelName = classEntity.GradeLevel.GradeLevelName,
            SchoolYearId = classEntity.SchoolYearId,
            SchoolYearName = classEntity.SchoolYear.SchoolYearName,
            ClassSize = classEntity.ClassSize,
            CurrentStudentCount = classEntity.ClassAssignments.Count,
            TeacherId = classEntity.TeacherId,
            TeacherName = classEntity.Teacher?.TeacherName
        };
    }

    public async Task<List<ClassDto>> GetClassesBySchoolYearAndGradeLevelAsync(string schoolYearId, string gradeLevelId)
    {
        var classes = await _context.Classes
            .Where(c => c.SchoolYearId == schoolYearId && c.GradeLevelId == gradeLevelId)
            .Include(c => c.GradeLevel)
            .Include(c => c.SchoolYear)
            .Include(c => c.Teacher)
            .Include(c => c.ClassAssignments)
            .OrderBy(c => c.ClassName)
            .Select(c => new ClassDto
            {
                ClassId = c.ClassId,
                ClassName = c.ClassName,
                GradeLevelId = c.GradeLevelId,
                GradeLevelName = c.GradeLevel.GradeLevelName,
                SchoolYearId = c.SchoolYearId,
                SchoolYearName = c.SchoolYear.SchoolYearName,
                ClassSize = c.ClassSize,
                CurrentStudentCount = c.ClassAssignments.Count,
                TeacherId = c.TeacherId,
                TeacherName = c.Teacher != null ? c.Teacher.TeacherName : null
            })
            .ToListAsync();

        return classes;
    }

    public async Task<ClassDto> CreateClassAsync(CreateClassDto createClassDto)
    {
        var classEntity = new Class
        {
            ClassId = createClassDto.ClassId,
            ClassName = createClassDto.ClassName,
            GradeLevelId = createClassDto.GradeLevelId,
            SchoolYearId = createClassDto.SchoolYearId,
            ClassSize = createClassDto.ClassSize,
            TeacherId = null
        };

        _context.Classes.Add(classEntity);
        await _context.SaveChangesAsync();

        return (await GetClassByIdAsync(classEntity.ClassId))!;
    }

    public async Task<ClassDto> UpdateClassAsync(string classId, UpdateClassDto updateClassDto)
    {
        var classEntity = await _context.Classes
            .Include(c => c.ClassAssignments)
            .Include(c => c.TeachingAssignments)
            .Include(c => c.Grades)
            .FirstOrDefaultAsync(c => c.ClassId == classId);

        if (classEntity == null)
            throw new KeyNotFoundException($"Class with ID {classId} not found");

        // If ClassId is being changed, we need to handle it specially
        if (classId != updateClassDto.ClassId)
        {
            // Check if new ID already exists
            var existingClass = await _context.Classes.FindAsync(updateClassDto.ClassId);
            if (existingClass != null)
                throw new InvalidOperationException($"Class with ID {updateClassDto.ClassId} already exists");

            // Check if there are related records - if so, we cannot change the ClassId
            if (classEntity.ClassAssignments.Any() || classEntity.TeachingAssignments.Any() || classEntity.Grades.Any())
            {
                throw new InvalidOperationException("Cannot change Class ID because there are related records (students, teaching assignments, or grades). Please remove those first.");
            }

            // Create new entity with new ID
            var newClass = new Class
            {
                ClassId = updateClassDto.ClassId,
                ClassName = updateClassDto.ClassName,
                GradeLevelId = updateClassDto.GradeLevelId,
                SchoolYearId = updateClassDto.SchoolYearId,
                ClassSize = updateClassDto.ClassSize,
                TeacherId = classEntity.TeacherId
            };

            // Remove old entity and add new one
            _context.Classes.Remove(classEntity);
            _context.Classes.Add(newClass);
            await _context.SaveChangesAsync();

            return (await GetClassByIdAsync(newClass.ClassId))!;
        }
        else
        {
            // Just update properties without changing ID
            classEntity.ClassName = updateClassDto.ClassName;
            classEntity.GradeLevelId = updateClassDto.GradeLevelId;
            classEntity.SchoolYearId = updateClassDto.SchoolYearId;
            classEntity.ClassSize = updateClassDto.ClassSize;

            await _context.SaveChangesAsync();

            return (await GetClassByIdAsync(classEntity.ClassId))!;
        }
    }

    public async Task<List<TeacherDto>> GetEligibleHomeroomTeachersAsync(string classId)
    {
        var classEntity = await _context.Classes.FindAsync(classId);
        if (classEntity == null)
            throw new KeyNotFoundException($"Class with ID {classId} not found");

        // Get teachers who teach at least one subject in this class
        var eligibleTeachers = await _context.TeachingAssignments
            .Where(ta => ta.ClassId == classId)
            .Select(ta => ta.Teacher)
            .Distinct()
            .Include(t => t.Subject)
            .Include(t => t.Department)
            .Select(t => new TeacherDto
            {
                TeacherId = t.TeacherId,
                UserId = t.UserId,
                TeacherName = t.TeacherName,
                Address = t.Address,
                PhoneNumber = t.PhoneNumber,
                SubjectId = t.SubjectId,
                DepartmentId = t.DepartmentId
            })
            .ToListAsync();

        return eligibleTeachers;
    }

    public async Task<ClassDto> AssignTeacherToClassAsync(string classId, AssignTeacherDto assignTeacherDto)
    {
        var classEntity = await _context.Classes.FindAsync(classId);
        if (classEntity == null)
            throw new KeyNotFoundException($"Class with ID {classId} not found");

        var teacher = await _context.Teachers.FindAsync(assignTeacherDto.TeacherId);
        if (teacher == null)
            throw new KeyNotFoundException($"Teacher with ID {assignTeacherDto.TeacherId} not found");

        // Verify that the teacher teaches at least one subject in this class
        var teachesInClass = await _context.TeachingAssignments
            .AnyAsync(ta => ta.ClassId == classId && ta.TeacherId == assignTeacherDto.TeacherId);

        if (!teachesInClass)
            throw new InvalidOperationException("The teacher must teach at least one subject in the class to be assigned as homeroom teacher");

        classEntity.TeacherId = assignTeacherDto.TeacherId;
        await _context.SaveChangesAsync();

        return (await GetClassByIdAsync(classEntity.ClassId))!;
    }

    public async Task<bool> DeleteClassAsync(string classId)
    {
        var classEntity = await _context.Classes.FindAsync(classId);
        if (classEntity == null)
            return false;

        _context.Classes.Remove(classEntity);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ClassExistsAsync(string classId)
    {
        return await _context.Classes.AnyAsync(c => c.ClassId == classId);
    }
}
