using Microsoft.EntityFrameworkCore;
using API.Data;
using API.DTOs;
using API.Models;

namespace API.Services;

public class SubjectService : ISubjectService
{
    private readonly ApplicationDbContext _context;

    public SubjectService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SubjectDto>> GetAllSubjectsAsync()
    {
        var subjectList = await _context.Subjects
            .OrderBy(s => s.SubjectId)
            .ToListAsync();

        return subjectList.Select(s => new SubjectDto
        {
            SubjectId = s.SubjectId,
            SubjectName = s.SubjectName,
            LessonCount = s.LessonCount,
            Coefficient = s.Coefficient
        }).ToList();
    }

    public async Task<SubjectDto?> GetSubjectByIdAsync(string subjectId)
    {
        var subject = await _context.Subjects.FindAsync(subjectId);
        if (subject == null) return null;

        return new SubjectDto
        {
            SubjectId = subject.SubjectId,
            SubjectName = subject.SubjectName,
            LessonCount = subject.LessonCount,
            Coefficient = subject.Coefficient
        };
    }

    public async Task<SubjectDto?> CreateSubjectAsync(CreateSubjectDto createDto)
    {
        // Check if subject name already exists
        var existingSubject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.SubjectName == createDto.SubjectName);

        if (existingSubject != null)
        {
            return null; // Duplicate name found
        }

        var subjectId = await GenerateNextSubjectIdAsync();

        var subject = new Subject
        {
            SubjectId = subjectId,
            SubjectName = createDto.SubjectName,
            LessonCount = createDto.LessonCount,
            Coefficient = createDto.Coefficient
        };

        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        return new SubjectDto
        {
            SubjectId = subject.SubjectId,
            SubjectName = subject.SubjectName,
            LessonCount = subject.LessonCount,
            Coefficient = subject.Coefficient
        };
    }

    private async Task<string> GenerateNextSubjectIdAsync()
    {
        var lastSubject = await _context.Subjects
            .OrderByDescending(s => s.SubjectId)
            .FirstOrDefaultAsync();

        if (lastSubject == null)
        {
            return "MH001";
        }

        var lastCode = lastSubject.SubjectId;
        if (lastCode.StartsWith("MH") && lastCode.Length > 2)
        {
            var numericPart = lastCode.Substring(2);
            if (int.TryParse(numericPart, out int number))
            {
                return $"MH{(number + 1):D3}"; // Format as MH001, MH002, etc.
            }
        }

        var count = await _context.Subjects.CountAsync();
        return $"MH{(count + 1):D3}";
    }

    public async Task<SubjectDto?> UpdateSubjectAsync(string subjectId, UpdateSubjectDto updateDto)
    {
        var subject = await _context.Subjects.FindAsync(subjectId);
        if (subject == null) return null;

        subject.SubjectName = updateDto.SubjectName;
        subject.LessonCount = updateDto.LessonCount;
        subject.Coefficient = updateDto.Coefficient;
        await _context.SaveChangesAsync();

        return new SubjectDto
        {
            SubjectId = subject.SubjectId,
            SubjectName = subject.SubjectName,
            LessonCount = subject.LessonCount,
            Coefficient = subject.Coefficient
        };
    }

    public async Task<bool> DeleteSubjectAsync(string subjectId)
    {
        var subject = await _context.Subjects
            .Include(s => s.Teachers)
            .Include(s => s.TeachingAssignments)
            .Include(s => s.Grades)
            .Include(s => s.StudentSubjectResults)
            .Include(s => s.ClassSubjectResults)
            .FirstOrDefaultAsync(s => s.SubjectId == subjectId);

        if (subject == null) return false;

        // Check if subject is being used in any related entities
        if (subject.Teachers.Any() || subject.TeachingAssignments.Any() ||
            subject.Grades.Any() || subject.StudentSubjectResults.Any() ||
            subject.ClassSubjectResults.Any())
        {
            return false;
        }

        _context.Subjects.Remove(subject);
        await _context.SaveChangesAsync();
        return true;
    }
}
