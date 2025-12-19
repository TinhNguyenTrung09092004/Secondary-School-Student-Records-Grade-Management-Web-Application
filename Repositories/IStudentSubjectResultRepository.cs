using API.Models;

namespace API.Repositories;

public interface IStudentSubjectResultRepository
{
    Task<IEnumerable<StudentSubjectResult>> GetAllAsync();
    Task<StudentSubjectResult?> GetByIdAsync(string studentId, string classId, string schoolYearId, string subjectId, string semesterId);
    Task<StudentSubjectResult> AddAsync(StudentSubjectResult entity);
    Task UpdateAsync(StudentSubjectResult entity);
    Task DeleteAsync(string studentId, string classId, string schoolYearId, string subjectId, string semesterId);
    Task<IEnumerable<StudentSubjectResult>> GetByStudentAsync(string studentId);
}
