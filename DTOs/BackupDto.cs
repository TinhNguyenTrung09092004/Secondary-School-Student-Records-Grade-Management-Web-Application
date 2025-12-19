namespace API.DTOs;

public class BackupScheduleDto
{
    public string BackupTime { get; set; } = "00:00";
    public bool IsEnabled { get; set; }
    public DateTime? LastBackupDate { get; set; }
}

public class BackupFileDto
{
    public string FileName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public long FileSizeBytes { get; set; }
    public string FilePath { get; set; } = string.Empty;
}

public class ManualBackupRequest
{
    public List<string> Tables { get; set; } = new();
}
