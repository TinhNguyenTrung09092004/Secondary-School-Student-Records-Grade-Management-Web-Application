using Microsoft.EntityFrameworkCore;
using API.Data;
using API.DTOs;
using API.Models;

namespace API.Services;

public class SemesterService : ISemesterService
{
    private readonly ApplicationDbContext _context;

    public SemesterService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SemesterDto>> GetAllSemestersAsync()
    {
        var semesterList = await _context.Semesters
            .OrderBy(s => s.SemesterId)
            .ToListAsync();

        return semesterList.Select(s => new SemesterDto
        {
            SemesterId = s.SemesterId,
            SemesterName = s.SemesterName,
            Coefficient = s.Coefficient
        }).ToList();
    }

    public async Task<SemesterDto?> GetSemesterByIdAsync(string semesterId)
    {
        var semester = await _context.Semesters.FindAsync(semesterId);
        if (semester == null) return null;

        return new SemesterDto
        {
            SemesterId = semester.SemesterId,
            SemesterName = semester.SemesterName,
            Coefficient = semester.Coefficient
        };
    }

    public async Task<SemesterDto?> CreateSemesterAsync(CreateSemesterDto createDto)
    {
        // Check if semester ID already exists
        var existingSemester = await _context.Semesters
            .FirstOrDefaultAsync(s => s.SemesterId == createDto.SemesterId);

        if (existingSemester != null)
        {
            return null; // Duplicate ID found
        }

        // Check if semester name already exists
        var existingName = await _context.Semesters
            .FirstOrDefaultAsync(s => s.SemesterName == createDto.SemesterName);

        if (existingName != null)
        {
            return null; // Duplicate name found
        }

        var semester = new Semester
        {
            SemesterId = createDto.SemesterId,
            SemesterName = createDto.SemesterName,
            Coefficient = createDto.Coefficient
        };

        _context.Semesters.Add(semester);
        await _context.SaveChangesAsync();

        return new SemesterDto
        {
            SemesterId = semester.SemesterId,
            SemesterName = semester.SemesterName,
            Coefficient = semester.Coefficient
        };
    }

    public async Task<SemesterDto?> UpdateSemesterAsync(string semesterId, UpdateSemesterDto updateDto)
    {
        var semester = await _context.Semesters
            .Include(s => s.Grades)
            .Include(s => s.StudentSubjectResults)
            .Include(s => s.ClassSubjectResults)
            .Include(s => s.ClassSemesterResults)
            .FirstOrDefaultAsync(s => s.SemesterId == semesterId);

        if (semester == null) return null;

        // Check if semester ID is changing
        if (semesterId != updateDto.SemesterId)
        {
            // Check if there are related records
            if (semester.Grades.Any() || semester.StudentSubjectResults.Any() ||
                semester.ClassSubjectResults.Any() || semester.ClassSemesterResults.Any())
            {
                throw new InvalidOperationException("Không thể thay đổi mã học kỳ vì học kỳ đang được sử dụng");
            }

            // Check if new ID already exists
            var existingId = await _context.Semesters
                .FirstOrDefaultAsync(s => s.SemesterId == updateDto.SemesterId);

            if (existingId != null)
            {
                return null; // Duplicate ID found
            }

            // Delete old entity and create new one
            _context.Semesters.Remove(semester);
            await _context.SaveChangesAsync();

            var newSemester = new Semester
            {
                SemesterId = updateDto.SemesterId,
                SemesterName = updateDto.SemesterName,
                Coefficient = updateDto.Coefficient
            };

            _context.Semesters.Add(newSemester);
            await _context.SaveChangesAsync();

            return new SemesterDto
            {
                SemesterId = newSemester.SemesterId,
                SemesterName = newSemester.SemesterName,
                Coefficient = newSemester.Coefficient
            };
        }
        else
        {
            // Just update name and coefficient
            // Check if new name conflicts with existing semester
            var existingName = await _context.Semesters
                .FirstOrDefaultAsync(s => s.SemesterName == updateDto.SemesterName && s.SemesterId != semesterId);

            if (existingName != null)
            {
                return null; // Duplicate name found
            }

            semester.SemesterName = updateDto.SemesterName;
            semester.Coefficient = updateDto.Coefficient;
            await _context.SaveChangesAsync();

            return new SemesterDto
            {
                SemesterId = semester.SemesterId,
                SemesterName = semester.SemesterName,
                Coefficient = semester.Coefficient
            };
        }
    }

    public async Task<bool> DeleteSemesterAsync(string semesterId)
    {
        var semester = await _context.Semesters
            .Include(s => s.Grades)
            .Include(s => s.StudentSubjectResults)
            .Include(s => s.ClassSubjectResults)
            .Include(s => s.ClassSemesterResults)
            .FirstOrDefaultAsync(s => s.SemesterId == semesterId);

        if (semester == null) return false;

        // Check if semester is being used in any related entities
        if (semester.Grades.Any() || semester.StudentSubjectResults.Any() ||
            semester.ClassSubjectResults.Any() || semester.ClassSemesterResults.Any())
        {
            return false;
        }

        _context.Semesters.Remove(semester);
        await _context.SaveChangesAsync();
        return true;
    }
}
