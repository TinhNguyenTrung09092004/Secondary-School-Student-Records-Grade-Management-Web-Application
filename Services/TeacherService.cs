using Microsoft.EntityFrameworkCore;
using API.Data;
using API.DTOs;
using API.Models;

namespace API.Services;

public class TeacherService : ITeacherService
{
    private readonly ApplicationDbContext _context;

    public TeacherService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TeacherDto>> GetAllTeachersAsync()
    {
        var teachers = await _context.Teachers
            .OrderBy(t => t.TeacherName)
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

        return teachers;
    }

    public async Task<TeacherDto?> GetTeacherProfileByUserIdAsync(string userId)
    {
        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(t => t.UserId == userId);

        if (teacher == null) return null;

        return new TeacherDto
        {
            TeacherId = teacher.TeacherId,
            UserId = teacher.UserId,
            TeacherName = teacher.TeacherName,
            Address = teacher.Address,
            PhoneNumber = teacher.PhoneNumber,
            SubjectId = teacher.SubjectId,
            DepartmentId = teacher.DepartmentId
        };
    }

    public async Task<TeacherDto?> CreateTeacherProfileAsync(string userId, CreateTeacherProfileDto createDto)
    {
        // Check if teacher ID already exists
        var existingTeacherById = await _context.Teachers
            .FirstOrDefaultAsync(t => t.TeacherId == createDto.TeacherId);

        if (existingTeacherById != null)
        {
            return null; // Duplicate teacher ID found
        }

        // Check if user already has a teacher profile
        var existingTeacherByUserId = await _context.Teachers
            .FirstOrDefaultAsync(t => t.UserId == userId);

        if (existingTeacherByUserId != null)
        {
            return null; // User already has a teacher profile
        }

        // Verify subject exists
        var subject = await _context.Subjects.FindAsync(createDto.SubjectId);
        if (subject == null)
        {
            return null; // Subject not found
        }

        // Verify department exists if provided
        if (!string.IsNullOrEmpty(createDto.DepartmentId))
        {
            var department = await _context.Departments.FindAsync(createDto.DepartmentId);
            if (department == null)
            {
                return null; // Department not found
            }
        }

        var teacher = new Teacher
        {
            TeacherId = createDto.TeacherId,
            UserId = userId,
            TeacherName = createDto.TeacherName,
            Address = createDto.Address,
            PhoneNumber = createDto.PhoneNumber,
            SubjectId = createDto.SubjectId,
            DepartmentId = createDto.DepartmentId
        };

        _context.Teachers.Add(teacher);
        await _context.SaveChangesAsync();

        return new TeacherDto
        {
            TeacherId = teacher.TeacherId,
            UserId = teacher.UserId,
            TeacherName = teacher.TeacherName,
            Address = teacher.Address,
            PhoneNumber = teacher.PhoneNumber,
            SubjectId = teacher.SubjectId,
            DepartmentId = teacher.DepartmentId
        };
    }

    public async Task<TeacherDto?> UpdateTeacherProfileAsync(string userId, UpdateTeacherProfileDto updateDto)
    {
        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(t => t.UserId == userId);

        if (teacher == null) return null;

        // Verify subject exists
        var subject = await _context.Subjects.FindAsync(updateDto.SubjectId);
        if (subject == null)
        {
            return null; // Subject not found
        }

        // Verify department exists if provided
        if (!string.IsNullOrEmpty(updateDto.DepartmentId))
        {
            var department = await _context.Departments.FindAsync(updateDto.DepartmentId);
            if (department == null)
            {
                return null; // Department not found
            }
        }

        teacher.TeacherName = updateDto.TeacherName;
        teacher.Address = updateDto.Address;
        teacher.PhoneNumber = updateDto.PhoneNumber;
        teacher.SubjectId = updateDto.SubjectId;
        teacher.DepartmentId = updateDto.DepartmentId;

        await _context.SaveChangesAsync();

        return new TeacherDto
        {
            TeacherId = teacher.TeacherId,
            UserId = teacher.UserId,
            TeacherName = teacher.TeacherName,
            Address = teacher.Address,
            PhoneNumber = teacher.PhoneNumber,
            SubjectId = teacher.SubjectId,
            DepartmentId = teacher.DepartmentId
        };
    }

    public async Task<bool> DeleteTeacherProfileAsync(string userId)
    {
        var teacher = await _context.Teachers
            .Include(t => t.Classes)
            .Include(t => t.TeachingAssignments)
            .FirstOrDefaultAsync(t => t.UserId == userId);

        if (teacher == null) return false;

        // Check if teacher has any related records
        if (teacher.Classes.Any() || teacher.TeachingAssignments.Any())
        {
            return false; // Cannot delete teacher with existing records
        }

        _context.Teachers.Remove(teacher);
        await _context.SaveChangesAsync();
        return true;
    }
}
