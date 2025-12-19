using API.DTOs;

namespace API.Services;

public interface ISemesterService
{
    Task<List<SemesterDto>> GetAllSemestersAsync();
    Task<SemesterDto?> GetSemesterByIdAsync(string semesterId);
    Task<SemesterDto?> CreateSemesterAsync(CreateSemesterDto createDto);
    Task<SemesterDto?> UpdateSemesterAsync(string semesterId, UpdateSemesterDto updateDto);
    Task<bool> DeleteSemesterAsync(string semesterId);
}
