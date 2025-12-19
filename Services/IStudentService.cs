using API.DTOs;

namespace API.Services;

public interface IStudentService
{
    Task<List<StudentDto>> GetAllStudentsAsync();
    Task<StudentDto?> GetStudentByIdAsync(string studentId);
    Task<StudentDto?> CreateStudentAsync(CreateStudentDto createDto);
    Task<StudentDto?> UpdateStudentAsync(string studentId, UpdateStudentDto updateDto);
    Task<bool> DeleteStudentAsync(string studentId);
}
