using Microsoft.EntityFrameworkCore;
using API.Data;
using API.DTOs;
using API.Models;

namespace API.Services;

public class OccupationService : IOccupationService
{
    private readonly ApplicationDbContext _context;

    public OccupationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<OccupationDto>> GetAllOccupationAsync()
    {
        var occupationList = await _context.Occupations
            .OrderBy(o => o.OccupationId)
            .ToListAsync();

        return occupationList.Select(o => new OccupationDto
        {
            OccupationId = o.OccupationId,
            OccupationName = o.OccupationName
        }).ToList();
    }

    public async Task<OccupationDto?> GetOccupationByIdAsync(string occupationId)
    {
        var occupation = await _context.Occupations.FindAsync(occupationId);
        if (occupation == null) return null;

        return new OccupationDto
        {
            OccupationId = occupation.OccupationId,
            OccupationName = occupation.OccupationName
        };
    }

    public async Task<OccupationDto?> CreateOccupationAsync(CreateOccupationDto createDto)
    {
        // Check if occupation name already exists
        var existingOccupation = await _context.Occupations
            .FirstOrDefaultAsync(o => o.OccupationName == createDto.OccupationName);

        if (existingOccupation != null)
        {
            return null; // Duplicate name found
        }

        var occupationId = await GenerateNextOccupationIdAsync();

        var occupation = new Occupation
        {
            OccupationId = occupationId,
            OccupationName = createDto.OccupationName
        };

        _context.Occupations.Add(occupation);
        await _context.SaveChangesAsync();

        return new OccupationDto
        {
            OccupationId = occupation.OccupationId,
            OccupationName = occupation.OccupationName
        };
    }

    private async Task<string> GenerateNextOccupationIdAsync()
    {
        var lastOccupation = await _context.Occupations
            .OrderByDescending(o => o.OccupationId)
            .FirstOrDefaultAsync();

        if (lastOccupation == null)
        {
            return "NN001";
        }

        var lastCode = lastOccupation.OccupationId;
        if (lastCode.StartsWith("NN") && lastCode.Length > 2)
        {
            var numericPart = lastCode.Substring(2);
            if (int.TryParse(numericPart, out int number))
            {
                return $"NN{(number + 1):D3}"; // Format as NN001, NN002, etc.
            }
        }

        var count = await _context.Occupations.CountAsync();
        return $"NN{(count + 1):D3}";
    }

    public async Task<OccupationDto?> UpdateOccupationAsync(string occupationId, UpdateOccupationDto updateDto)
    {
        var occupation = await _context.Occupations.FindAsync(occupationId);
        if (occupation == null) return null;

        occupation.OccupationName = updateDto.OccupationName;
        await _context.SaveChangesAsync();

        return new OccupationDto
        {
            OccupationId = occupation.OccupationId,
            OccupationName = occupation.OccupationName
        };
    }

    public async Task<bool> DeleteOccupationAsync(string occupationId)
    {
        var occupation = await _context.Occupations
            .Include(o => o.StudentFathers)
            .Include(o => o.StudentMothers)
            .FirstOrDefaultAsync(o => o.OccupationId == occupationId);

        if (occupation == null) return false;

        // Check if occupation is used by any students (father or mother)
        if (occupation.StudentFathers.Any() || occupation.StudentMothers.Any())
        {
            return false;
        }

        _context.Occupations.Remove(occupation);
        await _context.SaveChangesAsync();
        return true;
    }
}
