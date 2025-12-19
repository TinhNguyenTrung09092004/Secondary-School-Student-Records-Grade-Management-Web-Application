using API.Data;
using API.Models;

namespace API.Repositories;

public class BackupScheduleRepository : RepositoryBase<BackupSchedule, int>, IBackupScheduleRepository
{
    public BackupScheduleRepository(ApplicationDbContext context) : base(context)
    {
    }
}
