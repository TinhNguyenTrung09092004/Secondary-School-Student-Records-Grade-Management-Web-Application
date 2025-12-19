using API.Data;
using API.Models;

namespace API.Repositories;

public class GradeLevelRepository : RepositoryBase<GradeLevel, string>, IGradeLevelRepository
{
    public GradeLevelRepository(ApplicationDbContext context) : base(context)
    {
    }
}
