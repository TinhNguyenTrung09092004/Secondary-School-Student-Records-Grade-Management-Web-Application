using Microsoft.Extensions.Hosting;

namespace API.Services;

public class AccountCleanupService : IHostedService, IDisposable
{
    private readonly IServiceScopeFactory _scopeFactory;
    private Timer? _timer;

    public AccountCleanupService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Run cleanup every day at midnight
        var now = DateTime.UtcNow;
        var nextMidnight = now.Date.AddDays(1);
        var timeUntilMidnight = nextMidnight - now;

        _timer = new Timer(async _ => await DoWork(), null, timeUntilMidnight, TimeSpan.FromDays(1));

        return Task.CompletedTask;
    }

    private async Task DoWork()
    {
        using var scope = _scopeFactory.CreateScope();
        var userManagementService = scope.ServiceProvider.GetRequiredService<IUserManagementService>();

        await userManagementService.PermanentlyDeleteExpiredAccountsAsync();

        Console.WriteLine($"[{DateTime.UtcNow}] Account cleanup completed");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
