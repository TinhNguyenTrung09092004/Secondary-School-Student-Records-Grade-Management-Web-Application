using API.Models;

namespace API.Repositories;

public interface IAuditLogRepository : IRepositoryInt<AuditLog>
{
    Task<IEnumerable<AuditLog>> GetByUserIdAsync(string userId);
    Task<IEnumerable<AuditLog>> GetByActionAsync(string action);
    Task<IEnumerable<AuditLog>> GetByEntityAsync(string entity);
    Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<AuditLog>> GetRecentLogsAsync(int count = 100);
}