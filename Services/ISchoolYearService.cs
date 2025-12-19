using API.DTOs;

namespace API.Services;

public interface ISchoolYearService
{
    Task<List<SchoolYearDto>> GetAllSchoolYearsAsync();
    Task<SchoolYearDto?> GetSchoolYearByIdAsync(string schoolYearId);
    Task<SchoolYearDto?> CreateSchoolYearAsync(CreateSchoolYearDto createDto);
    Task<SchoolYearDto?> UpdateSchoolYearAsync(string schoolYearId, UpdateSchoolYearDto updateDto);
    Task<bool> DeleteSchoolYearAsync(string schoolYearId);
}
