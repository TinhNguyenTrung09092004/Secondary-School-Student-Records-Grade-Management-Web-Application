using Microsoft.EntityFrameworkCore;
using API.Data;
using API.DTOs;
using API.Models;

namespace API.Services;

public class StudentService : IStudentService
{
    private readonly ApplicationDbContext _context;

    public StudentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<StudentDto>> GetAllStudentsAsync()
    {
        var students = await _context.Students
            .OrderBy(s => s.StudentId)
            .ToListAsync();

        return students.Select(s => new StudentDto
        {
            StudentId = s.StudentId,
            FullName = s.FullName,
            Gender = s.Gender,
            DateOfBirth = s.DateOfBirth,
            Address = s.Address,
            Email = s.Email,
            EthnicityId = s.EthnicityId,
            ReligionId = s.ReligionId,
            FatherName = s.FatherName,
            FatherOccupationId = s.FatherOccupationId,
            MotherName = s.MotherName,
            MotherOccupationId = s.MotherOccupationId
        }).ToList();
    }

    public async Task<StudentDto?> GetStudentByIdAsync(string studentId)
    {
        var student = await _context.Students.FindAsync(studentId);
        if (student == null) return null;

        return new StudentDto
        {
            StudentId = student.StudentId,
            FullName = student.FullName,
            Gender = student.Gender,
            DateOfBirth = student.DateOfBirth,
            Address = student.Address,
            Email = student.Email,
            EthnicityId = student.EthnicityId,
            ReligionId = student.ReligionId,
            FatherName = student.FatherName,
            FatherOccupationId = student.FatherOccupationId,
            MotherName = student.MotherName,
            MotherOccupationId = student.MotherOccupationId
        };
    }

    public async Task<StudentDto?> CreateStudentAsync(CreateStudentDto createDto)
    {
        // Check if student ID already exists
        var existingStudentById = await _context.Students
            .FirstOrDefaultAsync(s => s.StudentId == createDto.StudentId);

        if (existingStudentById != null)
        {
            return null; // Duplicate student ID found
        }

        // Check if email already exists
        var existingStudentByEmail = await _context.Students
            .FirstOrDefaultAsync(s => s.Email == createDto.Email);

        if (existingStudentByEmail != null)
        {
            return null; // Duplicate email found
        }

        var student = new Student
        {
            StudentId = createDto.StudentId,
            FullName = createDto.FullName,
            Gender = createDto.Gender,
            DateOfBirth = createDto.DateOfBirth,
            Address = createDto.Address,
            Email = createDto.Email,
            EthnicityId = createDto.EthnicityId,
            ReligionId = createDto.ReligionId,
            FatherName = createDto.FatherName,
            FatherOccupationId = createDto.FatherOccupationId,
            MotherName = createDto.MotherName,
            MotherOccupationId = createDto.MotherOccupationId
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        return new StudentDto
        {
            StudentId = student.StudentId,
            FullName = student.FullName,
            Gender = student.Gender,
            DateOfBirth = student.DateOfBirth,
            Address = student.Address,
            Email = student.Email,
            EthnicityId = student.EthnicityId,
            ReligionId = student.ReligionId,
            FatherName = student.FatherName,
            FatherOccupationId = student.FatherOccupationId,
            MotherName = student.MotherName,
            MotherOccupationId = student.MotherOccupationId
        };
    }

    public async Task<StudentDto?> UpdateStudentAsync(string studentId, UpdateStudentDto updateDto)
    {
        var student = await _context.Students.FindAsync(studentId);
        if (student == null) return null;

        // Check if email is being changed to an existing email
        var existingStudent = await _context.Students
            .FirstOrDefaultAsync(s => s.Email == updateDto.Email && s.StudentId != studentId);

        if (existingStudent != null)
        {
            return null; // Duplicate email found
        }

        student.FullName = updateDto.FullName;
        student.Gender = updateDto.Gender;
        student.DateOfBirth = updateDto.DateOfBirth;
        student.Address = updateDto.Address;
        student.Email = updateDto.Email;
        student.EthnicityId = updateDto.EthnicityId;
        student.ReligionId = updateDto.ReligionId;
        student.FatherName = updateDto.FatherName;
        student.FatherOccupationId = updateDto.FatherOccupationId;
        student.MotherName = updateDto.MotherName;
        student.MotherOccupationId = updateDto.MotherOccupationId;

        await _context.SaveChangesAsync();

        return new StudentDto
        {
            StudentId = student.StudentId,
            FullName = student.FullName,
            Gender = student.Gender,
            DateOfBirth = student.DateOfBirth,
            Address = student.Address,
            Email = student.Email,
            EthnicityId = student.EthnicityId,
            ReligionId = student.ReligionId,
            FatherName = student.FatherName,
            FatherOccupationId = student.FatherOccupationId,
            MotherName = student.MotherName,
            MotherOccupationId = student.MotherOccupationId
        };
    }

    public async Task<bool> DeleteStudentAsync(string studentId)
    {
        var student = await _context.Students
            .Include(s => s.ClassAssignments)
            .Include(s => s.Grades)
            .Include(s => s.StudentSubjectResults)
            .Include(s => s.StudentYearResults)
            .FirstOrDefaultAsync(s => s.StudentId == studentId);

        if (student == null) return false;

        // Check if student has any records
        if (student.ClassAssignments.Any() || student.Grades.Any() ||
            student.StudentSubjectResults.Any() || student.StudentYearResults.Any())
        {
            return false; // Cannot delete student with existing records
        }

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
        return true;
    }
}
