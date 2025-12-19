using API.DTOs;

namespace API.Services;

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentResponseDto>> GetAllDepartmentsAsync();
    Task<DepartmentDetailDto?> GetDepartmentByIdAsync(string id);
    Task<DepartmentResponseDto?> CreateDepartmentAsync(CreateDepartmentDto createDto);
    Task<DepartmentResponseDto?> UpdateDepartmentAsync(string id, UpdateDepartmentDto updateDto);
    Task<bool> DeleteDepartmentAsync(string id);
}
