using API.Models;

namespace API.Repositories;

public interface IStudentRepository : IRepository<Student>
{
    Task<IEnumerable<Student>> GetByClassAsync(string classId);
}
