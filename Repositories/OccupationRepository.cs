using API.Data;
using API.Models;

namespace API.Repositories;

public class OccupationRepository : RepositoryBase<Occupation, string>, IOccupationRepository
{
    public OccupationRepository(ApplicationDbContext context) : base(context)
    {
    }
}
