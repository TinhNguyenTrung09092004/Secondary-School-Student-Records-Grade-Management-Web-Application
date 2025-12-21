using API.Models;
using API.Repositories;
using System.Text.Json;

namespace API.Services;

public class AuditService : IAuditService
{
    private readonly IAuditLogRepository _auditLogRepository;

    public AuditService(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task LogAsync(
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
        string? userAgent = null)
    {
        var auditLog = new AuditLog
        {
            PerformedByUserId = performedByUserId,
            PerformedByEmail = performedByEmail,
            Action = action,
            Entity = entity,
            TargetUserId = targetUserId,
            TargetEmail = targetEmail,
            TargetIdentifier = targetIdentifier,
            OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
            NewValues = newValues != null ? JsonSerializer.Serialize(newValues) : null,
            AdditionalInfo = additionalInfo,
            PerformedAt = DateTime.UtcNow,
            IpAddress = ipAddress,
            UserAgent = userAgent
        };

        await _auditLogRepository.AddAsync(auditLog);
    }

    public async Task<IEnumerable<AuditLog>> GetAllLogsAsync()
    {
        return await _auditLogRepository.GetAllAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetLogsByUserIdAsync(string userId)
    {
        return await _auditLogRepository.GetByUserIdAsync(userId);
    }

    public async Task<IEnumerable<AuditLog>> GetLogsByActionAsync(string action)
    {
        return await _auditLogRepository.GetByActionAsync(action);
    }

    public async Task<IEnumerable<AuditLog>> GetLogsByEntityAsync(string entity)
    {
        return await _auditLogRepository.GetByEntityAsync(entity);
    }

    public async Task<IEnumerable<AuditLog>> GetLogsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _auditLogRepository.GetByDateRangeAsync(startDate, endDate);
    }

    public async Task<IEnumerable<AuditLog>> GetRecentLogsAsync(int count = 100)
    {
        return await _auditLogRepository.GetRecentLogsAsync(count);
    }
}
