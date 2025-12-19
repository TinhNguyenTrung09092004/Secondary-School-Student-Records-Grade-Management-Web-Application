using API.DTOs;

namespace API.Services;

public interface IAcademicPerformanceService
{
    Task<IEnumerable<AcademicPerformanceDto>> GetAllAcademicPerformancesAsync();
    Task<AcademicPerformanceDto?> GetAcademicPerformanceByIdAsync(string id);
    Task<AcademicPerformanceDto> CreateAcademicPerformanceAsync(CreateAcademicPerformanceDto createDto);
    Task<AcademicPerformanceDto> UpdateAcademicPerformanceAsync(string id, UpdateAcademicPerformanceDto updateDto);
    Task DeleteAcademicPerformanceAsync(string id);
}
