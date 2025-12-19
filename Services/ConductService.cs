using Microsoft.EntityFrameworkCore;
using API.Data;
using API.DTOs;
using API.Models;

namespace API.Services;

public class ConductService : IConductService
{
    private readonly ApplicationDbContext _context;

    public ConductService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ConductDto>> GetAllConductAsync()
    {
        var conductList = await _context.Set<Conduct>()
            .OrderBy(c => c.ConductId)
            .ToListAsync();

        return conductList.Select(c => new ConductDto
        {
            ConductId = c.ConductId,
            ConductName = c.ConductName
        }).ToList();
    }

    public async Task<ConductDto?> GetConductByIdAsync(string conductId)
    {
        var conduct = await _context.Set<Conduct>().FindAsync(conductId);
        if (conduct == null) return null;

        return new ConductDto
        {
            ConductId = conduct.ConductId,
            ConductName = conduct.ConductName
        };
    }

    public async Task<ConductDto?> CreateConductAsync(CreateConductDto createDto)
    {
        // Check if conduct name already exists
        var existingConduct = await _context.Set<Conduct>()
            .FirstOrDefaultAsync(c => c.ConductName == createDto.ConductName);

        if (existingConduct != null)
        {
            return null; // Duplicate name found
        }

        var conductId = await GenerateNextConductIdAsync();

        var conduct = new Conduct
        {
            ConductId = conductId,
            ConductName = createDto.ConductName
        };

        _context.Set<Conduct>().Add(conduct);
        await _context.SaveChangesAsync();

        return new ConductDto
        {
            ConductId = conduct.ConductId,
            ConductName = conduct.ConductName
        };
    }

    private async Task<string> GenerateNextConductIdAsync()
    {
        var lastConduct = await _context.Set<Conduct>()
            .OrderByDescending(c => c.ConductId)
            .FirstOrDefaultAsync();

        if (lastConduct == null)
        {
            return "HL001";
        }

        var lastCode = lastConduct.ConductId;
        if (lastCode.StartsWith("HL") && lastCode.Length > 2)
        {
            var numericPart = lastCode.Substring(2);
            if (int.TryParse(numericPart, out int number))
            {
                return $"HL{(number + 1):D3}"; // Format as HL001, HL002, etc.
            }
        }

        var count = await _context.Set<Conduct>().CountAsync();
        return $"HL{(count + 1):D3}";
    }

    public async Task<ConductDto?> UpdateConductAsync(string conductId, UpdateConductDto updateDto)
    {
        var conduct = await _context.Set<Conduct>().FindAsync(conductId);
        if (conduct == null) return null;

        conduct.ConductName = updateDto.ConductName;
        await _context.SaveChangesAsync();

        return new ConductDto
        {
            ConductId = conduct.ConductId,
            ConductName = conduct.ConductName
        };
    }

    public async Task<bool> DeleteConductAsync(string conductId)
    {
        var conduct = await _context.Set<Conduct>()
            .Include(c => c.StudentYearResults)
            .FirstOrDefaultAsync(c => c.ConductId == conductId);

        if (conduct == null) return false;

        if (conduct.StudentYearResults.Any())
        {
            return false;
        }

        _context.Set<Conduct>().Remove(conduct);
        await _context.SaveChangesAsync();
        return true;
    }
}