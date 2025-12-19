using API.Data;
using API.Models;

namespace API.Repositories;

public class SchoolYearRepository : RepositoryBase<SchoolYear, string>, ISchoolYearRepository
{
    public SchoolYearRepository(ApplicationDbContext context) : base(context)
    {
    }
}
