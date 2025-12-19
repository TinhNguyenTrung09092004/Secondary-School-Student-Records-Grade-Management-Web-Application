using API.DTOs;

namespace API.Services;

public interface IClassAssignmentService
{
    Task<ClassInfoDto?> GetClassInfoAsync(string schoolYearId, string gradeLevelId, string classId);
    Task<List<StudentInClassDto>> GetStudentsInClassAsync(string schoolYearId, string gradeLevelId, string classId);
    Task<List<StudentInClassDto>> GetAvailableStudentsAsync(string schoolYearId, string gradeLevelId);
    Task<bool> AssignStudentToClassAsync(AssignStudentToClassDto assignDto);
    Task<bool> BulkAssignStudentsToClassAsync(BulkAssignStudentsDto bulkAssignDto);
    Task<bool> RemoveStudentFromClassAsync(RemoveStudentFromClassDto removeDto);
    Task<List<ClassAssignmentDto>> GetStudentClassHistoryAsync(string studentId);
}
