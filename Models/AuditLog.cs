namespace API.Models;

public class AuditLog
{
    public int Id { get; set; }

    // Who performed the action
    public string? PerformedByUserId { get; set; }
    public string? PerformedByEmail { get; set; }

    // What action was performed
    public string Action { get; set; } = string.Empty;
    public string Entity { get; set; } = string.Empty; // e.g., "User", "Role", "Student", "Grade"

    // Who/what was affected
    public string? TargetUserId { get; set; }
    public string? TargetEmail { get; set; }
    public string? TargetIdentifier { get; set; } // Generic identifier for non-user entities

    // Details of what changed
    public string? OldValues { get; set; } // JSON string of old values
    public string? NewValues { get; set; } // JSON string of new values
    public string? AdditionalInfo { get; set; } // Any extra context

    // When it happened
    public DateTime PerformedAt { get; set; } = DateTime.UtcNow;

    // Where it came from
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}
