using API.DTOs;

namespace API.Services;

public interface ISubjectService
{
    Task<List<SubjectDto>> GetAllSubjectsAsync();
    Task<SubjectDto?> GetSubjectByIdAsync(string subjectId);
    Task<SubjectDto?> CreateSubjectAsync(CreateSubjectDto createDto);
    Task<SubjectDto?> UpdateSubjectAsync(string subjectId, UpdateSubjectDto updateDto);
    Task<bool> DeleteSubjectAsync(string subjectId);
}
