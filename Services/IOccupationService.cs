using API.DTOs;

namespace API.Services;

public interface IOccupationService
{
    Task<List<OccupationDto>> GetAllOccupationAsync();
    Task<OccupationDto?> GetOccupationByIdAsync(string occupationId);
    Task<OccupationDto?> CreateOccupationAsync(CreateOccupationDto createDto);
    Task<OccupationDto?> UpdateOccupationAsync(string occupationId, UpdateOccupationDto updateDto);
    Task<bool> DeleteOccupationAsync(string occupationId);
}
