using API.Data;
using API.Models;

namespace API.Repositories;

public class ConductRepository : RepositoryBase<Conduct, string>, IConductRepository
{
    public ConductRepository(ApplicationDbContext context) : base(context)
    {
    }
}
