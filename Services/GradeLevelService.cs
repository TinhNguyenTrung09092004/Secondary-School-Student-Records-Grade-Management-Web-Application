using Microsoft.EntityFrameworkCore;
using API.Data;
using API.DTOs;
using API.Models;

namespace API.Services;

public class GradeLevelService : IGradeLevelService
{
    private readonly ApplicationDbContext _context;

    public GradeLevelService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<GradeLevelDto>> GetAllGradeLevelsAsync()
    {
        var gradeLevels = await _context.GradeLevels
            .OrderBy(g => g.GradeLevelId)
            .ToListAsync();

        return gradeLevels.Select(g => new GradeLevelDto
        {
            GradeLevelId = g.GradeLevelId,
            GradeLevelName = g.GradeLevelName
        }).ToList();
    }

    public async Task<GradeLevelDto?> GetGradeLevelByIdAsync(string gradeLevelId)
    {
        var gradeLevel = await _context.GradeLevels.FindAsync(gradeLevelId);
        if (gradeLevel == null) return null;

        return new GradeLevelDto
        {
            GradeLevelId = gradeLevel.GradeLevelId,
            GradeLevelName = gradeLevel.GradeLevelName
        };
    }

    public async Task<GradeLevelDto?> CreateGradeLevelAsync(CreateGradeLevelDto createDto)
    {
        // Check if grade level ID already exists
        var existingGradeLevel = await _context.GradeLevels
            .FirstOrDefaultAsync(g => g.GradeLevelId == createDto.GradeLevelId);

        if (existingGradeLevel != null)
        {
            return null; // Duplicate grade level ID found
        }

        var gradeLevel = new GradeLevel
        {
            GradeLevelId = createDto.GradeLevelId,
            GradeLevelName = createDto.GradeLevelName
        };

        _context.GradeLevels.Add(gradeLevel);
        await _context.SaveChangesAsync();

        return new GradeLevelDto
        {
            GradeLevelId = gradeLevel.GradeLevelId,
            GradeLevelName = gradeLevel.GradeLevelName
        };
    }

    public async Task<GradeLevelDto?> UpdateGradeLevelAsync(string gradeLevelId, UpdateGradeLevelDto updateDto)
    {
        var gradeLevel = await _context.GradeLevels.FindAsync(gradeLevelId);
        if (gradeLevel == null) return null;

        // If ID is being changed, check if new ID already exists
        if (gradeLevelId != updateDto.GradeLevelId)
        {
            var existingGradeLevel = await _context.GradeLevels
                .FirstOrDefaultAsync(g => g.GradeLevelId == updateDto.GradeLevelId);

            if (existingGradeLevel != null)
            {
                return null; // New ID already exists
            }

            // Delete old record and create new one with new ID
            _context.GradeLevels.Remove(gradeLevel);

            var newGradeLevel = new GradeLevel
            {
                GradeLevelId = updateDto.GradeLevelId,
                GradeLevelName = updateDto.GradeLevelName
            };

            _context.GradeLevels.Add(newGradeLevel);
            await _context.SaveChangesAsync();

            return new GradeLevelDto
            {
                GradeLevelId = newGradeLevel.GradeLevelId,
                GradeLevelName = newGradeLevel.GradeLevelName
            };
        }

        // If ID is not changing, just update the name
        gradeLevel.GradeLevelName = updateDto.GradeLevelName;
        await _context.SaveChangesAsync();

        return new GradeLevelDto
        {
            GradeLevelId = gradeLevel.GradeLevelId,
            GradeLevelName = gradeLevel.GradeLevelName
        };
    }

    public async Task<bool> DeleteGradeLevelAsync(string gradeLevelId)
    {
        var gradeLevel = await _context.GradeLevels
            .Include(g => g.Classes)
            .Include(g => g.ClassAssignments)
            .FirstOrDefaultAsync(g => g.GradeLevelId == gradeLevelId);

        if (gradeLevel == null) return false;

        // Check if grade level has any related records
        if (gradeLevel.Classes.Any() || gradeLevel.ClassAssignments.Any())
        {
            return false; // Cannot delete grade level with existing records
        }

        _context.GradeLevels.Remove(gradeLevel);
        await _context.SaveChangesAsync();
        return true;
    }
}
