using Microsoft.EntityFrameworkCore;
using API.Data;
using API.DTOs;
using API.Models;

namespace API.Services;

public class EthnicityService : IEthnicityService
{
    private readonly ApplicationDbContext _context;

    public EthnicityService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<EthnicityDto>> GetAllEthnicityAsync()
    {
        var ethnicityList = await _context.Ethnicities
            .OrderBy(d => d.EthnicityId)
            .ToListAsync();

        return ethnicityList.Select(d => new EthnicityDto
        {
            EthnicityId = d.EthnicityId,
            EthnicityName = d.EthnicityName
        }).ToList();
    }

    public async Task<EthnicityDto?> GetEthnicityByIdAsync(string ethnicityId)
    {
        var ethnicity = await _context.Ethnicities.FindAsync(ethnicityId);
        if (ethnicity == null) return null;

        return new EthnicityDto
        {
            EthnicityId = ethnicity.EthnicityId,
            EthnicityName = ethnicity.EthnicityName
        };
    }

    public async Task<EthnicityDto?> CreateEthnicityAsync(CreateEthnicityDto createDto)
    {
        // Check if ethnicity name already exists
        var existingEthnicity = await _context.Ethnicities
            .FirstOrDefaultAsync(e => e.EthnicityName == createDto.EthnicityName);

        if (existingEthnicity != null)
        {
            return null; // Duplicate name found
        }

        var ethnicityId = await GenerateNextEthnicityIdAsync();

        var ethnicity = new Ethnicity
        {
            EthnicityId = ethnicityId,
            EthnicityName = createDto.EthnicityName
        };

        _context.Ethnicities.Add(ethnicity);
        await _context.SaveChangesAsync();

        return new EthnicityDto
        {
            EthnicityId = ethnicity.EthnicityId,
            EthnicityName = ethnicity.EthnicityName
        };
    }

    private async Task<string> GenerateNextEthnicityIdAsync()
    {
        var lastEthnicity = await _context.Ethnicities
            .OrderByDescending(d => d.EthnicityId)
            .FirstOrDefaultAsync();

        if (lastEthnicity == null)
        {
            return "DT001";
        }

        var lastCode = lastEthnicity.EthnicityId;
        if (lastCode.StartsWith("DT") && lastCode.Length > 2)
        {
            var numericPart = lastCode.Substring(2);
            if (int.TryParse(numericPart, out int number))
            {
                return $"DT{(number + 1):D3}"; // Format as DT001, DT002, etc.
            }
        }

        var count = await _context.Ethnicities.CountAsync();
        return $"DT{(count + 1):D3}";
    }

    public async Task<EthnicityDto?> UpdateEthnicityAsync(string ethnicityId, UpdateEthnicityDto updateDto)
    {
        var ethnicity = await _context.Ethnicities.FindAsync(ethnicityId);
        if (ethnicity == null) return null;

        ethnicity.EthnicityName = updateDto.EthnicityName;
        await _context.SaveChangesAsync();

        return new EthnicityDto
        {
            EthnicityId = ethnicity.EthnicityId,
            EthnicityName = ethnicity.EthnicityName
        };
    }

    public async Task<bool> DeleteEthnicityAsync(string ethnicityId)
    {
        var ethnicity = await _context.Ethnicities
            .Include(d => d.Students)
            .FirstOrDefaultAsync(d => d.EthnicityId == ethnicityId);

        if (ethnicity == null) return false;

        if (ethnicity.Students.Any())
        {
            return false;
        }

        _context.Ethnicities.Remove(ethnicity);
        await _context.SaveChangesAsync();
        return true;
    }
}
