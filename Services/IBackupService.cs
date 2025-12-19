using API.DTOs;

namespace API.Services;

public interface IBackupService
{
    Task<List<string>> GetAllTablesAsync();
    Task<string> CreateBackupAsync(List<string> tables);
    Task<List<BackupFileDto>> GetBackupFilesAsync();
    Task<BackupScheduleDto> GetBackupScheduleAsync();
    Task UpdateBackupScheduleAsync(BackupScheduleDto dto);
    Task<string> GetBackupFilePathAsync(string fileName);
    Task RestoreBackupAsync(string fileName);
}
