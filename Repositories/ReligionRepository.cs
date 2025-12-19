using API.Data;
using API.Models;

namespace API.Repositories;

public class ReligionRepository : RepositoryBase<Religion, string>, IReligionRepository
{
    public ReligionRepository(ApplicationDbContext context) : base(context)
    {
    }
}
