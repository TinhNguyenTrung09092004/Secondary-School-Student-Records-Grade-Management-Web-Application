using API.DTOs;

namespace API.Services;

public interface ITeachingAssignmentService
{
    Task<IEnumerable<DepartmentTeacherDto>> GetDepartmentTeachersAsync(string userId);
    Task<IEnumerable<TeachingAssignmentDto>> GetAllTeachingAssignmentsAsync();
    Task<TeachingAssignmentDto?> CreateTeachingAssignmentAsync(string userId, CreateTeachingAssignmentDto createDto);
    Task<TeachingAssignmentDto?> UpdateTeachingAssignmentAsync(int id, string userId, UpdateTeachingAssignmentDto updateDto);
    Task<bool> DeleteTeachingAssignmentAsync(int id, string userId);
    Task<bool> IsDepartmentHeadAsync(string userId);
}
