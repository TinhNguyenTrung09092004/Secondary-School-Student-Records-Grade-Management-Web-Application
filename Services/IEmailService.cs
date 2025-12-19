namespace API.Services;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string email, string resetLink);
    Task SendAccountSetupEmailAsync(string email, string setupLink);
    Task SendAccountDeletionScheduledEmailAsync(string email, DateTime deletionDate);
    Task SendAccountDeletionCancelledEmailAsync(string email);
    Task SendAccountLockedEmailAsync(string email);
    Task SendAccountUnlockedEmailAsync(string email);
}
