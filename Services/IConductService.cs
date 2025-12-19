using API.DTOs;

namespace API.Services;

public interface IConductService
{
    Task<List<ConductDto>> GetAllConductAsync();
    Task<ConductDto?> GetConductByIdAsync(string conductId);
    Task<ConductDto?> CreateConductAsync(CreateConductDto createDto);
    Task<ConductDto?> UpdateConductAsync(string conductId, UpdateConductDto updateDto);
    Task<bool> DeleteConductAsync(string conductId);
}