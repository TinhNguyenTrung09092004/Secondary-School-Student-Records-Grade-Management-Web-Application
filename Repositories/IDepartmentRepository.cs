using API.Models;

namespace API.Repositories;

public interface IDepartmentRepository : IRepository<Department>
{
    Task<Department?> GetByIdWithDetailsAsync(string id);
    Task<IEnumerable<Department>> GetAllWithDetailsAsync();
}
