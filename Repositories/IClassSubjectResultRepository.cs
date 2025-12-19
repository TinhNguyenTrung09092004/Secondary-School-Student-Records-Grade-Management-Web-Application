using API.Models;

namespace API.Repositories;

public interface IClassSubjectResultRepository
{
    Task<IEnumerable<ClassSubjectResult>> GetAllAsync();
    Task<ClassSubjectResult?> GetByIdAsync(string classId, string schoolYearId, string subjectId, string semesterId);
    Task<ClassSubjectResult> AddAsync(ClassSubjectResult entity);
    Task UpdateAsync(ClassSubjectResult entity);
    Task DeleteAsync(string classId, string schoolYearId, string subjectId, string semesterId);
    Task<IEnumerable<ClassSubjectResult>> GetByClassAsync(string classId);
}
