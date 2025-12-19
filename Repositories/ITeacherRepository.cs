using API.Models;

namespace API.Repositories;

public interface ITeacherRepository : IRepository<Teacher>
{
    Task<Teacher?> GetByUserIdAsync(string userId);
    Task<IEnumerable<Teacher>> GetTeachersByDepartmentIdAsync(string departmentId);
    Task<IEnumerable<Teacher>> GetAllWithSubjectAsync();
}
