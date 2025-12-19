using API.DTOs;

namespace API.Services;

public interface ITeacherService
{
    Task<List<TeacherDto>> GetAllTeachersAsync();
    Task<TeacherDto?> GetTeacherProfileByUserIdAsync(string userId);
    Task<TeacherDto?> CreateTeacherProfileAsync(string userId, CreateTeacherProfileDto createDto);
    Task<TeacherDto?> UpdateTeacherProfileAsync(string userId, UpdateTeacherProfileDto updateDto);
    Task<bool> DeleteTeacherProfileAsync(string userId);
}
