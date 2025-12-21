using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Services;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AuditLogController : ControllerBase
{
    private readonly IAuditService _auditService;

    public AuditLogController(IAuditService auditService)
    {
        _auditService = auditService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllLogs()
    {
        var logs = await _auditService.GetAllLogsAsync();
        var logDtos = logs.Select(log => new AuditLogDto
        {
            Id = log.Id,
            PerformedByUserId = log.PerformedByUserId,
            PerformedByEmail = log.PerformedByEmail,
            Action = log.Action,
            Entity = log.Entity,
            TargetUserId = log.TargetUserId,
            TargetEmail = log.TargetEmail,
            TargetIdentifier = log.TargetIdentifier,
            OldValues = log.OldValues,
            NewValues = log.NewValues,
            AdditionalInfo = log.AdditionalInfo,
            PerformedAt = log.PerformedAt,
            IpAddress = log.IpAddress,
            UserAgent = log.UserAgent
        }).ToList();

        return Ok(logDtos);
    }

    [HttpGet("recent")]
    public async Task<IActionResult> GetRecentLogs([FromQuery] int count = 100)
    {
        var logs = await _auditService.GetRecentLogsAsync(count);
        var logDtos = logs.Select(log => new AuditLogDto
        {
            Id = log.Id,
            PerformedByUserId = log.PerformedByUserId,
            PerformedByEmail = log.PerformedByEmail,
            Action = log.Action,
            Entity = log.Entity,
            TargetUserId = log.TargetUserId,
            TargetEmail = log.TargetEmail,
            TargetIdentifier = log.TargetIdentifier,
            OldValues = log.OldValues,
            NewValues = log.NewValues,
            AdditionalInfo = log.AdditionalInfo,
            PerformedAt = log.PerformedAt,
            IpAddress = log.IpAddress,
            UserAgent = log.UserAgent
        }).ToList();

        return Ok(logDtos);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetLogsByUser(string userId)
    {
        var logs = await _auditService.GetLogsByUserIdAsync(userId);
        var logDtos = logs.Select(log => new AuditLogDto
        {
            Id = log.Id,
            PerformedByUserId = log.PerformedByUserId,
            PerformedByEmail = log.PerformedByEmail,
            Action = log.Action,
            Entity = log.Entity,
            TargetUserId = log.TargetUserId,
            TargetEmail = log.TargetEmail,
            TargetIdentifier = log.TargetIdentifier,
            OldValues = log.OldValues,
            NewValues = log.NewValues,
            AdditionalInfo = log.AdditionalInfo,
            PerformedAt = log.PerformedAt,
            IpAddress = log.IpAddress,
            UserAgent = log.UserAgent
        }).ToList();

        return Ok(logDtos);
    }

    [HttpGet("action/{action}")]
    public async Task<IActionResult> GetLogsByAction(string action)
    {
        var logs = await _auditService.GetLogsByActionAsync(action);
        var logDtos = logs.Select(log => new AuditLogDto
        {
            Id = log.Id,
            PerformedByUserId = log.PerformedByUserId,
            PerformedByEmail = log.PerformedByEmail,
            Action = log.Action,
            Entity = log.Entity,
            TargetUserId = log.TargetUserId,
            TargetEmail = log.TargetEmail,
            TargetIdentifier = log.TargetIdentifier,
            OldValues = log.OldValues,
            NewValues = log.NewValues,
            AdditionalInfo = log.AdditionalInfo,
            PerformedAt = log.PerformedAt,
            IpAddress = log.IpAddress,
            UserAgent = log.UserAgent
        }).ToList();

        return Ok(logDtos);
    }

    [HttpGet("entity/{entity}")]
    public async Task<IActionResult> GetLogsByEntity(string entity)
    {
        var logs = await _auditService.GetLogsByEntityAsync(entity);
        var logDtos = logs.Select(log => new AuditLogDto
        {
            Id = log.Id,
            PerformedByUserId = log.PerformedByUserId,
            PerformedByEmail = log.PerformedByEmail,
            Action = log.Action,
            Entity = log.Entity,
            TargetUserId = log.TargetUserId,
            TargetEmail = log.TargetEmail,
            TargetIdentifier = log.TargetIdentifier,
            OldValues = log.OldValues,
            NewValues = log.NewValues,
            AdditionalInfo = log.AdditionalInfo,
            PerformedAt = log.PerformedAt,
            IpAddress = log.IpAddress,
            UserAgent = log.UserAgent
        }).ToList();

        return Ok(logDtos);
    }

    [HttpGet("daterange")]
    public async Task<IActionResult> GetLogsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var logs = await _auditService.GetLogsByDateRangeAsync(startDate, endDate);
        var logDtos = logs.Select(log => new AuditLogDto
        {
            Id = log.Id,
            PerformedByUserId = log.PerformedByUserId,
            PerformedByEmail = log.PerformedByEmail,
            Action = log.Action,
            Entity = log.Entity,
            TargetUserId = log.TargetUserId,
            TargetEmail = log.TargetEmail,
            TargetIdentifier = log.TargetIdentifier,
            OldValues = log.OldValues,
            NewValues = log.NewValues,
            AdditionalInfo = log.AdditionalInfo,
            PerformedAt = log.PerformedAt,
            IpAddress = log.IpAddress,
            UserAgent = log.UserAgent
        }).ToList();

        return Ok(logDtos);
    }

    [HttpPost("filter")]
    public async Task<IActionResult> FilterLogs([FromBody] AuditLogFilterDto filter)
    {
        IEnumerable<Models.AuditLog> logs;

        if (!string.IsNullOrEmpty(filter.UserId))
        {
            logs = await _auditService.GetLogsByUserIdAsync(filter.UserId);
        }
        else if (!string.IsNullOrEmpty(filter.Action))
        {
            logs = await _auditService.GetLogsByActionAsync(filter.Action);
        }
        else if (!string.IsNullOrEmpty(filter.Entity))
        {
            logs = await _auditService.GetLogsByEntityAsync(filter.Entity);
        }
        else if (filter.StartDate.HasValue && filter.EndDate.HasValue)
        {
            logs = await _auditService.GetLogsByDateRangeAsync(filter.StartDate.Value, filter.EndDate.Value);
        }
        else
        {
            logs = await _auditService.GetRecentLogsAsync(filter.Limit ?? 100);
        }

        var logDtos = logs.Select(log => new AuditLogDto
        {
            Id = log.Id,
            PerformedByUserId = log.PerformedByUserId,
            PerformedByEmail = log.PerformedByEmail,
            Action = log.Action,
            Entity = log.Entity,
            TargetUserId = log.TargetUserId,
            TargetEmail = log.TargetEmail,
            TargetIdentifier = log.TargetIdentifier,
            OldValues = log.OldValues,
            NewValues = log.NewValues,
            AdditionalInfo = log.AdditionalInfo,
            PerformedAt = log.PerformedAt,
            IpAddress = log.IpAddress,
            UserAgent = log.UserAgent
        }).ToList();

        return Ok(logDtos);
    }
}
