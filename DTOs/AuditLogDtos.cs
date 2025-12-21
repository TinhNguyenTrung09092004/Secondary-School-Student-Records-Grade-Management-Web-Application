namespace API.DTOs;

public class AuditLogDto
{
    public int Id { get; set; }
    public string? PerformedByUserId { get; set; }
    public string? PerformedByEmail { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Entity { get; set; } = string.Empty;
    public string? TargetUserId { get; set; }
    public string? TargetEmail { get; set; }
    public string? TargetIdentifier { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? AdditionalInfo { get; set; }
    public DateTime PerformedAt { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}

public class AuditLogFilterDto
{
    public string? UserId { get; set; }
    public string? Action { get; set; }
    public string? Entity { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? Limit { get; set; } = 100;
}