using API.Data;
using API.Models;

namespace API.Repositories;

public class ClassRepository : RepositoryBase<Class, string>, IClassRepository
{
    public ClassRepository(ApplicationDbContext context) : base(context)
    {
    }
}
