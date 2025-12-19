using Microsoft.EntityFrameworkCore;
using API.Data;
using API.DTOs;
using API.Models;

namespace API.Services;

public class SchoolYearService : ISchoolYearService
{
    private readonly ApplicationDbContext _context;

    public SchoolYearService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SchoolYearDto>> GetAllSchoolYearsAsync()
    {
        var schoolYears = await _context.SchoolYears
            .OrderBy(s => s.SchoolYearId)
            .ToListAsync();

        return schoolYears.Select(s => new SchoolYearDto
        {
            SchoolYearId = s.SchoolYearId,
            SchoolYearName = s.SchoolYearName
        }).ToList();
    }

    public async Task<SchoolYearDto?> GetSchoolYearByIdAsync(string schoolYearId)
    {
        var schoolYear = await _context.SchoolYears.FindAsync(schoolYearId);
        if (schoolYear == null) return null;

        return new SchoolYearDto
        {
            SchoolYearId = schoolYear.SchoolYearId,
            SchoolYearName = schoolYear.SchoolYearName
        };
    }

    public async Task<SchoolYearDto?> CreateSchoolYearAsync(CreateSchoolYearDto createDto)
    {
        // Check if school year ID already exists
        var existingSchoolYear = await _context.SchoolYears
            .FirstOrDefaultAsync(s => s.SchoolYearId == createDto.SchoolYearId);

        if (existingSchoolYear != null)
        {
            return null; // Duplicate school year ID found
        }

        var schoolYear = new SchoolYear
        {
            SchoolYearId = createDto.SchoolYearId,
            SchoolYearName = createDto.SchoolYearName
        };

        _context.SchoolYears.Add(schoolYear);
        await _context.SaveChangesAsync();

        return new SchoolYearDto
        {
            SchoolYearId = schoolYear.SchoolYearId,
            SchoolYearName = schoolYear.SchoolYearName
        };
    }

    public async Task<SchoolYearDto?> UpdateSchoolYearAsync(string schoolYearId, UpdateSchoolYearDto updateDto)
    {
        var schoolYear = await _context.SchoolYears.FindAsync(schoolYearId);
        if (schoolYear == null) return null;

        // If ID is being changed, check if new ID already exists
        if (schoolYearId != updateDto.SchoolYearId)
        {
            var existingSchoolYear = await _context.SchoolYears
                .FirstOrDefaultAsync(s => s.SchoolYearId == updateDto.SchoolYearId);

            if (existingSchoolYear != null)
            {
                return null; // New ID already exists
            }

            // Delete old record and create new one with new ID
            _context.SchoolYears.Remove(schoolYear);

            var newSchoolYear = new SchoolYear
            {
                SchoolYearId = updateDto.SchoolYearId,
                SchoolYearName = updateDto.SchoolYearName
            };

            _context.SchoolYears.Add(newSchoolYear);
            await _context.SaveChangesAsync();

            return new SchoolYearDto
            {
                SchoolYearId = newSchoolYear.SchoolYearId,
                SchoolYearName = newSchoolYear.SchoolYearName
            };
        }

        // If ID is not changing, just update the name
        schoolYear.SchoolYearName = updateDto.SchoolYearName;
        await _context.SaveChangesAsync();

        return new SchoolYearDto
        {
            SchoolYearId = schoolYear.SchoolYearId,
            SchoolYearName = schoolYear.SchoolYearName
        };
    }

    public async Task<bool> DeleteSchoolYearAsync(string schoolYearId)
    {
        var schoolYear = await _context.SchoolYears
            .Include(s => s.Classes)
            .Include(s => s.ClassAssignments)
            .Include(s => s.TeachingAssignments)
            .Include(s => s.Grades)
            .Include(s => s.StudentSubjectResults)
            .Include(s => s.StudentYearResults)
            .Include(s => s.ClassSubjectResults)
            .Include(s => s.ClassSemesterResults)
            .FirstOrDefaultAsync(s => s.SchoolYearId == schoolYearId);

        if (schoolYear == null) return false;

        // Check if school year has any related records
        if (schoolYear.Classes.Any() || schoolYear.ClassAssignments.Any() ||
            schoolYear.TeachingAssignments.Any() || schoolYear.Grades.Any() ||
            schoolYear.StudentSubjectResults.Any() || schoolYear.StudentYearResults.Any() ||
            schoolYear.ClassSubjectResults.Any() || schoolYear.ClassSemesterResults.Any())
        {
            return false; // Cannot delete school year with existing records
        }

        _context.SchoolYears.Remove(schoolYear);
        await _context.SaveChangesAsync();
        return true;
    }
}
