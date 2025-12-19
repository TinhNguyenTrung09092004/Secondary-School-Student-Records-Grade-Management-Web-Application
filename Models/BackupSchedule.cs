using System.ComponentModel.DataAnnotations;

namespace API.Models;

public class BackupSchedule
{
    [Key]
    public int Id { get; set; }

    public string BackupTime { get; set; } = "00:00"; // Daily backup time in HH:mm format

    public bool IsEnabled { get; set; } = false;

    public DateTime? LastBackupDate { get; set; }
}
