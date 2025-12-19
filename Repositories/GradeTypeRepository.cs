using API.Data;
using API.Models;

namespace API.Repositories;

public class GradeTypeRepository : RepositoryBase<GradeType, string>, IGradeTypeRepository
{
    public GradeTypeRepository(ApplicationDbContext context) : base(context)
    {
    }
}
