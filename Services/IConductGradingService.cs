using API.DTOs;

namespace API.Services;

public interface IConductGradingService
{
    Task<List<ClassForConductGradingDto>> GetTeacherClassesAsync(string teacherId);
    Task<List<StudentConductDto>> GetStudentsForConductGradingAsync(string classId, string schoolYearId, string teacherId);
    Task<bool> UpdateStudentConductAsync(UpdateStudentConductDto updateDto, string teacherId);
}
