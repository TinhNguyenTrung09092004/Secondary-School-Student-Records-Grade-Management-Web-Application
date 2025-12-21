using API.Models;

namespace API.Services;

public interface IAuditService
{
    Task LogAsync(
        string? performedByUserId,
        string? performedByEmail,
        string action,
        string entity,
        string? targetUserId = null,
        string? targetEmail = null,
        string? targetIdentifier = null,
        object? oldValues = null,
        object? newValues = null,
        string? additionalInfo = null,
        string? ipAddress = null,
        string? userAgent = null
    );

    Task<IEnumerable<AuditLog>> GetAllLogsAsync();
    Task<IEnumerable<AuditLog>> GetLogsByUserIdAsync(string userId);
    Task<IEnumerable<AuditLog>> GetLogsByActionAsync(string action);
    Task<IEnumerable<AuditLog>> GetLogsByEntityAsync(string entity);
    Task<IEnumerable<AuditLog>> GetLogsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<AuditLog>> GetRecentLogsAsync(int count = 100);
}
