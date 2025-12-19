using Microsoft.EntityFrameworkCore;
using API.Data;
using API.DTOs;
using API.Models;

namespace API.Services;

public class ClassAssignmentService : IClassAssignmentService
{
    private readonly ApplicationDbContext _context;

    public ClassAssignmentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ClassInfoDto?> GetClassInfoAsync(string schoolYearId, string gradeLevelId, string classId)
    {
        var classEntity = await _context.Classes
            .Include(c => c.GradeLevel)
            .Include(c => c.SchoolYear)
            .Include(c => c.Teacher)
            .Include(c => c.ClassAssignments)
            .FirstOrDefaultAsync(c => c.ClassId == classId &&
                                     c.SchoolYearId == schoolYearId &&
                                     c.GradeLevelId == gradeLevelId);

        if (classEntity == null) return null;

        return new ClassInfoDto
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

    public async Task<List<StudentInClassDto>> GetStudentsInClassAsync(string schoolYearId, string gradeLevelId, string classId)
    {
        var students = await _context.ClassAssignments
            .Where(ca => ca.SchoolYearId == schoolYearId &&
                        ca.GradeLevelId == gradeLevelId &&
                        ca.ClassId == classId)
            .Include(ca => ca.Student)
            .Select(ca => new StudentInClassDto
            {
                StudentId = ca.Student.StudentId,
                FullName = ca.Student.FullName,
                Email = ca.Student.Email,
                Gender = ca.Student.Gender
            })
            .OrderBy(s => s.FullName)
            .ToListAsync();

        return students;
    }

    public async Task<List<StudentInClassDto>> GetAvailableStudentsAsync(string schoolYearId, string gradeLevelId)
    {
        // Get all students who are NOT assigned to any class for this school year and grade level
        var assignedStudentIds = await _context.ClassAssignments
            .Where(ca => ca.SchoolYearId == schoolYearId && ca.GradeLevelId == gradeLevelId)
            .Select(ca => ca.StudentId)
            .ToListAsync();

        var availableStudents = await _context.Students
            .Where(s => !assignedStudentIds.Contains(s.StudentId))
            .Select(s => new StudentInClassDto
            {
                StudentId = s.StudentId,
                FullName = s.FullName,
                Email = s.Email,
                Gender = s.Gender
            })
            .OrderBy(s => s.FullName)
            .ToListAsync();

        return availableStudents;
    }

    public async Task<bool> AssignStudentToClassAsync(AssignStudentToClassDto assignDto)
    {
        // Check if class exists
        var classEntity = await _context.Classes
            .Include(c => c.ClassAssignments)
            .FirstOrDefaultAsync(c => c.ClassId == assignDto.ClassId &&
                                     c.SchoolYearId == assignDto.SchoolYearId &&
                                     c.GradeLevelId == assignDto.GradeLevelId);

        if (classEntity == null) return false;

        // Check if student exists
        var student = await _context.Students.FindAsync(assignDto.StudentId);
        if (student == null) return false;

        // Check if class is full
        if (classEntity.ClassAssignments.Count >= classEntity.ClassSize)
        {
            return false; // Class is full
        }

        // Check if student is already assigned to a class for this school year and grade level
        var existingAssignment = await _context.ClassAssignments
            .FirstOrDefaultAsync(ca => ca.StudentId == assignDto.StudentId &&
                                      ca.SchoolYearId == assignDto.SchoolYearId &&
                                      ca.GradeLevelId == assignDto.GradeLevelId);

        if (existingAssignment != null)
        {
            return false; // Student already assigned
        }

        var classAssignment = new ClassAssignment
        {
            SchoolYearId = assignDto.SchoolYearId,
            GradeLevelId = assignDto.GradeLevelId,
            ClassId = assignDto.ClassId,
            StudentId = assignDto.StudentId
        };

        _context.ClassAssignments.Add(classAssignment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> BulkAssignStudentsToClassAsync(BulkAssignStudentsDto bulkAssignDto)
    {
        // Check if class exists
        var classEntity = await _context.Classes
            .Include(c => c.ClassAssignments)
            .FirstOrDefaultAsync(c => c.ClassId == bulkAssignDto.ClassId &&
                                     c.SchoolYearId == bulkAssignDto.SchoolYearId &&
                                     c.GradeLevelId == bulkAssignDto.GradeLevelId);

        if (classEntity == null) return false;

        // Check if class has enough capacity
        var currentCount = classEntity.ClassAssignments.Count;
        var newStudentsCount = bulkAssignDto.StudentIds.Count;

        if (currentCount + newStudentsCount > classEntity.ClassSize)
        {
            return false; // Not enough space in class
        }

        // Get already assigned student IDs for this year/grade
        var assignedStudentIds = await _context.ClassAssignments
            .Where(ca => ca.SchoolYearId == bulkAssignDto.SchoolYearId &&
                        ca.GradeLevelId == bulkAssignDto.GradeLevelId)
            .Select(ca => ca.StudentId)
            .ToListAsync();

        // Filter out students who are already assigned
        var studentsToAssign = bulkAssignDto.StudentIds
            .Where(id => !assignedStudentIds.Contains(id))
            .Distinct()
            .ToList();

        if (!studentsToAssign.Any()) return false;

        // Verify all students exist
        var existingStudents = await _context.Students
            .Where(s => studentsToAssign.Contains(s.StudentId))
            .Select(s => s.StudentId)
            .ToListAsync();

        if (existingStudents.Count != studentsToAssign.Count)
        {
            return false; // Some students don't exist
        }

        // Create assignments
        var assignments = studentsToAssign.Select(studentId => new ClassAssignment
        {
            SchoolYearId = bulkAssignDto.SchoolYearId,
            GradeLevelId = bulkAssignDto.GradeLevelId,
            ClassId = bulkAssignDto.ClassId,
            StudentId = studentId
        }).ToList();

        _context.ClassAssignments.AddRange(assignments);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveStudentFromClassAsync(RemoveStudentFromClassDto removeDto)
    {
        var assignment = await _context.ClassAssignments
            .FirstOrDefaultAsync(ca => ca.SchoolYearId == removeDto.SchoolYearId &&
                                      ca.GradeLevelId == removeDto.GradeLevelId &&
                                      ca.ClassId == removeDto.ClassId &&
                                      ca.StudentId == removeDto.StudentId);

        if (assignment == null) return false;

        _context.ClassAssignments.Remove(assignment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<ClassAssignmentDto>> GetStudentClassHistoryAsync(string studentId)
    {
        var history = await _context.ClassAssignments
            .Where(ca => ca.StudentId == studentId)
            .Include(ca => ca.SchoolYear)
            .Include(ca => ca.GradeLevel)
            .Include(ca => ca.Class)
            .Include(ca => ca.Student)
            .Select(ca => new ClassAssignmentDto
            {
                SchoolYearId = ca.SchoolYearId,
                SchoolYearName = ca.SchoolYear.SchoolYearName,
                GradeLevelId = ca.GradeLevelId,
                GradeLevelName = ca.GradeLevel.GradeLevelName,
                ClassId = ca.ClassId,
                ClassName = ca.Class.ClassName,
                StudentId = ca.StudentId,
                StudentName = ca.Student.FullName
            })
            .OrderByDescending(ca => ca.SchoolYearId)
            .ToListAsync();

        return history;
    }
}
