using API.Data;
using API.Models;

namespace API.Repositories;

public class SubjectRepository : RepositoryBase<Subject, string>, ISubjectRepository
{
    public SubjectRepository(ApplicationDbContext context) : base(context)
    {
    }
}
