using API.DTOs;

namespace API.Services;

public interface IReligionService
{
    Task<List<ReligionDto>> GetAllReligionAsync();
    Task<ReligionDto?> GetReligionByIdAsync(string religionId);
    Task<ReligionDto?> CreateReligionAsync(CreateReligionDto createDto);
    Task<ReligionDto?> UpdateReligionAsync(string religionId, UpdateReligionDto updateDto);
    Task<bool> DeleteReligionAsync(string religionId);
}
