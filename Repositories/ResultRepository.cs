using API.Data;
using API.Models;

namespace API.Repositories;

public class ResultRepository : RepositoryBase<Result, string>, IResultRepository
{
    public ResultRepository(ApplicationDbContext context) : base(context)
    {
    }
}
