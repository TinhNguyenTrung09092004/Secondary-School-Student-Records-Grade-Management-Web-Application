using API.Models;

namespace API.Repositories;

public interface IClassSemesterResultRepository
{
    Task<IEnumerable<ClassSemesterResult>> GetAllAsync();
    Task<ClassSemesterResult?> GetByIdAsync(string classId, string schoolYearId, string semesterId);
    Task<ClassSemesterResult> AddAsync(ClassSemesterResult entity);
    Task UpdateAsync(ClassSemesterResult entity);
    Task DeleteAsync(string classId, string schoolYearId, string semesterId);
    Task<IEnumerable<ClassSemesterResult>> GetByClassAsync(string classId);
}
