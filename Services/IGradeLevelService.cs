using API.DTOs;

namespace API.Services;

public interface IGradeLevelService
{
    Task<List<GradeLevelDto>> GetAllGradeLevelsAsync();
    Task<GradeLevelDto?> GetGradeLevelByIdAsync(string gradeLevelId);
    Task<GradeLevelDto?> CreateGradeLevelAsync(CreateGradeLevelDto createDto);
    Task<GradeLevelDto?> UpdateGradeLevelAsync(string gradeLevelId, UpdateGradeLevelDto updateDto);
    Task<bool> DeleteGradeLevelAsync(string gradeLevelId);
}
