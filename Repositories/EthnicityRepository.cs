using API.Data;
using API.Models;

namespace API.Repositories;

public class EthnicityRepository : RepositoryBase<Ethnicity, string>, IEthnicityRepository
{
    public EthnicityRepository(ApplicationDbContext context) : base(context)
    {
    }
}
