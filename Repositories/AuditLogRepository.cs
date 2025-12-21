using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class AuditLogRepository : RepositoryBase<AuditLog, int>, IAuditLogRepository
{
    public AuditLogRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(string userId)
    {
        return await _dbSet
            .Where(a => a.PerformedByUserId == userId || a.TargetUserId == userId)
            .OrderByDescending(a => a.PerformedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetByActionAsync(string action)
    {
        return await _dbSet
            .Where(a => a.Action == action)
            .OrderByDescending(a => a.PerformedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetByEntityAsync(string entity)
    {
        return await _dbSet
            .Where(a => a.Entity == entity)
            .OrderByDescending(a => a.PerformedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(a => a.PerformedAt >= startDate && a.PerformedAt <= endDate)
            .OrderByDescending(a => a.PerformedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetRecentLogsAsync(int count = 100)
    {
        return await _dbSet
            .OrderByDescending(a => a.PerformedAt)
            .Take(count)
            .ToListAsync();
    }
}
