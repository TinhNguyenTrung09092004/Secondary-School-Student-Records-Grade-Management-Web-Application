using API.Models;

namespace API.Repositories;

public interface IStudentSemesterResultRepository
{
    Task<IEnumerable<StudentSemesterResult>> GetAllAsync();
    Task<StudentSemesterResult?> GetByIdAsync(string studentId, string classId, string schoolYearId, string semesterId);
    Task<StudentSemesterResult> AddAsync(StudentSemesterResult entity);
    Task UpdateAsync(StudentSemesterResult entity);
    Task DeleteAsync(string studentId, string classId, string schoolYearId, string semesterId);
    Task<IEnumerable<StudentSemesterResult>> GetByStudentAsync(string studentId);
}