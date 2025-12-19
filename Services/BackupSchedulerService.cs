using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class BackupSchedulerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackupSchedulerService> _logger;

    public BackupSchedulerService(IServiceProvider serviceProvider, ILogger<BackupSchedulerService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Backup Scheduler Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var backupService = scope.ServiceProvider.GetRequiredService<IBackupService>();

                var schedule = await context.Set<BackupSchedule>().FirstOrDefaultAsync(stoppingToken);

                if (schedule != null && schedule.IsEnabled)
                {
                    var nowLocal = DateTime.Now;
                    var scheduledTime = TimeSpan.Parse(schedule.BackupTime);
                    var todayScheduledDateTime = nowLocal.Date.Add(scheduledTime);

                    if (nowLocal >= todayScheduledDateTime &&
                        (schedule.LastBackupDate == null || schedule.LastBackupDate.Value.ToLocalTime().Date < nowLocal.Date))
                    {
                        _logger.LogInformation("Starting scheduled backup at {Time}", nowLocal);

                        var allTables = await backupService.GetAllTablesAsync();

                        var fileName = await backupService.CreateBackupAsync(allTables);

                        schedule.LastBackupDate = DateTime.Now.ToUniversalTime();
                        await context.SaveChangesAsync(stoppingToken);

                        _logger.LogInformation("Scheduled backup completed: {FileName}", fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in backup scheduler");
            }

            // Check every 1 minute
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }

        _logger.LogInformation("Backup Scheduler Service stopped");
    }
}
