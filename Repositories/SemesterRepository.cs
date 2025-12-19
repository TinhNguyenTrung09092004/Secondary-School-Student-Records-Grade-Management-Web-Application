using API.Data;
using API.Models;

namespace API.Repositories;

public class SemesterRepository : RepositoryBase<Semester, string>, ISemesterRepository
{
    public SemesterRepository(ApplicationDbContext context) : base(context)
    {
    }
}
