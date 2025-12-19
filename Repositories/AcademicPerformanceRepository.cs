using API.Data;
using API.Models;

namespace API.Repositories;

public class AcademicPerformanceRepository : RepositoryBase<AcademicPerformance, string>, IAcademicPerformanceRepository
{
    public AcademicPerformanceRepository(ApplicationDbContext context) : base(context)
    {
    }
}
