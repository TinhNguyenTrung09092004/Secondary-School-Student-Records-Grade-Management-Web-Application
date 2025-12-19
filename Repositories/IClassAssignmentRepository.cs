using API.Models;

namespace API.Repositories;

public interface IClassAssignmentRepository
{
    Task<IEnumerable<ClassAssignment>> GetAllAsync();
    Task<ClassAssignment?> GetByIdAsync(string schoolYearId, string gradeLevelId, string classId, string studentId);
    Task<ClassAssignment> AddAsync(ClassAssignment entity);
    Task UpdateAsync(ClassAssignment entity);
    Task DeleteAsync(string schoolYearId, string gradeLevelId, string classId, string studentId);
    Task<IEnumerable<ClassAssignment>> GetByClassAsync(string classId);
    Task<IEnumerable<ClassAssignment>> GetByStudentAsync(string studentId);
    Task<IEnumerable<ClassAssignment>> GetStudentsByClassIdAsync(string classId);
}
