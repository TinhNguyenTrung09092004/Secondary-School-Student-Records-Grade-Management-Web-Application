using Microsoft.EntityFrameworkCore;
using API.Data;
using API.DTOs;
using API.Models;

namespace API.Services;

public class ReligionService : IReligionService
{
    private readonly ApplicationDbContext _context;

    public ReligionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ReligionDto>> GetAllReligionAsync()
    {
        var religionList = await _context.Religions
            .OrderBy(r => r.ReligionId)
            .ToListAsync();

        return religionList.Select(r => new ReligionDto
        {
            ReligionId = r.ReligionId,
            ReligionName = r.ReligionName
        }).ToList();
    }

    public async Task<ReligionDto?> GetReligionByIdAsync(string religionId)
    {
        var religion = await _context.Religions.FindAsync(religionId);
        if (religion == null) return null;

        return new ReligionDto
        {
            ReligionId = religion.ReligionId,
            ReligionName = religion.ReligionName
        };
    }

    public async Task<ReligionDto?> CreateReligionAsync(CreateReligionDto createDto)
    {
        // Check if religion name already exists
        var existingReligion = await _context.Religions
            .FirstOrDefaultAsync(r => r.ReligionName == createDto.ReligionName);

        if (existingReligion != null)
        {
            return null; // Duplicate name found
        }

        var religionId = await GenerateNextReligionIdAsync();

        var religion = new Religion
        {
            ReligionId = religionId,
            ReligionName = createDto.ReligionName
        };

        _context.Religions.Add(religion);
        await _context.SaveChangesAsync();

        return new ReligionDto
        {
            ReligionId = religion.ReligionId,
            ReligionName = religion.ReligionName
        };
    }

    private async Task<string> GenerateNextReligionIdAsync()
    {
        var lastReligion = await _context.Religions
            .OrderByDescending(r => r.ReligionId)
            .FirstOrDefaultAsync();

        if (lastReligion == null)
        {
            return "TG001";
        }

        var lastCode = lastReligion.ReligionId;
        if (lastCode.StartsWith("TG") && lastCode.Length > 2)
        {
            var numericPart = lastCode.Substring(2);
            if (int.TryParse(numericPart, out int number))
            {
                return $"TG{(number + 1):D3}"; // Format as TG001, TG002, etc.
            }
        }

        var count = await _context.Religions.CountAsync();
        return $"TG{(count + 1):D3}";
    }

    public async Task<ReligionDto?> UpdateReligionAsync(string religionId, UpdateReligionDto updateDto)
    {
        var religion = await _context.Religions.FindAsync(religionId);
        if (religion == null) return null;

        religion.ReligionName = updateDto.ReligionName;
        await _context.SaveChangesAsync();

        return new ReligionDto
        {
            ReligionId = religion.ReligionId,
            ReligionName = religion.ReligionName
        };
    }

    public async Task<bool> DeleteReligionAsync(string religionId)
    {
        var religion = await _context.Religions
            .Include(r => r.Students)
            .FirstOrDefaultAsync(r => r.ReligionId == religionId);

        if (religion == null) return false;

        if (religion.Students.Any())
        {
            return false;
        }

        _context.Religions.Remove(religion);
        await _context.SaveChangesAsync();
        return true;
    }
}
