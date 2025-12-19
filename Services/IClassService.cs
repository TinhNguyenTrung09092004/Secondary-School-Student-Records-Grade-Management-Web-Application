using API.DTOs;

namespace API.Services;

public interface IClassService
{
    Task<List<ClassDto>> GetAllClassesAsync();
    Task<ClassDto?> GetClassByIdAsync(string classId);
    Task<List<ClassDto>> GetClassesBySchoolYearAndGradeLevelAsync(string schoolYearId, string gradeLevelId);
    Task<ClassDto> CreateClassAsync(CreateClassDto createClassDto);
    Task<ClassDto> UpdateClassAsync(string classId, UpdateClassDto updateClassDto);
    Task<List<TeacherDto>> GetEligibleHomeroomTeachersAsync(string classId);
    Task<ClassDto> AssignTeacherToClassAsync(string classId, AssignTeacherDto assignTeacherDto);
    Task<bool> DeleteClassAsync(string classId);
    Task<bool> ClassExistsAsync(string classId);
}
