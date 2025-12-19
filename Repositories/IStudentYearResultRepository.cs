using API.Models;

namespace API.Repositories;

public interface IStudentYearResultRepository
{
    Task<IEnumerable<StudentYearResult>> GetAllAsync();
    Task<StudentYearResult?> GetByIdAsync(string studentId, string classId, string schoolYearId);
    Task<StudentYearResult> AddAsync(StudentYearResult entity);
    Task UpdateAsync(StudentYearResult entity);
    Task DeleteAsync(string studentId, string classId, string schoolYearId);
    Task<IEnumerable<StudentYearResult>> GetByStudentAsync(string studentId);
}
