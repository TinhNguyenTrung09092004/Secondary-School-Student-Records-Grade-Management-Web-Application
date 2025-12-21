using Microsoft.AspNetCore.Identity;
using API.DTOs;
using API.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace API.Services;

public class UserManagementService : IUserManagementService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly IAuditService _auditService;

    public UserManagementService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IEmailService emailService,
        IConfiguration configuration,
        IAuditService auditService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _emailService = emailService;
        _configuration = configuration;
        _auditService = auditService;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDtos.Add(new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                UserName = user.UserName!,
                EmailConfirmed = user.EmailConfirmed,
                Roles = roles.ToList(),
                LockoutEnd = user.LockoutEnd?.DateTime,
                IsLockedOut = user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow,
                IsAccountSetupComplete = user.IsAccountSetupComplete,
                ScheduledDeletionDate = user.ScheduledDeletionDate
            });
        }

        return userDtos;
    }

    public async Task<UserDto?> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || user.IsDeleted) return null;

        var roles = await _userManager.GetRolesAsync(user);
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            UserName = user.UserName!,
            EmailConfirmed = user.EmailConfirmed,
            Roles = roles.ToList(),
            LockoutEnd = user.LockoutEnd?.DateTime,
            IsLockedOut = user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow,
            IsAccountSetupComplete = user.IsAccountSetupComplete,
            ScheduledDeletionDate = user.ScheduledDeletionDate
        };
    }

    public async Task<UserDto?> CreateUserAsync(CreateUserDto createUserDto, string? currentUserId = null, string? currentUserEmail = null)
    {
        var setupToken = GenerateSecureToken();

        var user = new ApplicationUser
        {
            UserName = createUserDto.Email,
            Email = createUserDto.Email,
            EmailConfirmed = true,
            IsAccountSetupComplete = false,
            AccountSetupToken = setupToken,
            AccountSetupTokenExpiry = DateTime.UtcNow.AddDays(7),
            IsDeleted = false
        };

        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded) return null;

        if (createUserDto.Roles.Any())
        {
            // Validate that all roles exist before assigning
            foreach (var roleName in createUserDto.Roles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    // Rollback user creation if invalid role
                    await _userManager.DeleteAsync(user);
                    return null;
                }
            }
            await _userManager.AddToRolesAsync(user, createUserDto.Roles);
        }

        var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL") ?? "http://localhost:4200";
        var setupLink = $"{frontendUrl}/validate-token?token={setupToken}&email={user.Email}&type=setup";
        await _emailService.SendAccountSetupEmailAsync(user.Email, setupLink);

        var roles = await _userManager.GetRolesAsync(user);

        // Log audit event
        await _auditService.LogAsync(
            performedByUserId: currentUserId,
            performedByEmail: currentUserEmail,
            action: "CreateUser",
            entity: "User",
            targetUserId: user.Id,
            targetEmail: user.Email,
            newValues: new { Email = user.Email, Roles = roles.ToList() },
            additionalInfo: "User account created and setup email sent"
        );

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            UserName = user.UserName!,
            EmailConfirmed = user.EmailConfirmed,
            Roles = roles.ToList(),
            LockoutEnd = user.LockoutEnd?.DateTime,
            IsLockedOut = false,
            IsAccountSetupComplete = user.IsAccountSetupComplete,
            ScheduledDeletionDate = user.ScheduledDeletionDate
        };
    }

    public async Task<UserDto?> UpdateUserAsync(string userId, UpdateUserDto updateUserDto, string? currentUserId = null, string? currentUserEmail = null)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || user.IsDeleted) return null;

        var oldEmail = user.Email;
        var currentRoles = await _userManager.GetRolesAsync(user);

        user.Email = updateUserDto.Email;
        user.UserName = updateUserDto.Email;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded) return null;

        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        if (updateUserDto.Roles.Any())
        {
            // Validate that all roles exist before assigning
            foreach (var roleName in updateUserDto.Roles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    // Restore previous roles if invalid role found
                    await _userManager.AddToRolesAsync(user, currentRoles);
                    return null;
                }
            }
            await _userManager.AddToRolesAsync(user, updateUserDto.Roles);
        }

        var roles = await _userManager.GetRolesAsync(user);

        // Log audit event
        await _auditService.LogAsync(
            performedByUserId: currentUserId,
            performedByEmail: currentUserEmail,
            action: "UpdateUser",
            entity: "User",
            targetUserId: user.Id,
            targetEmail: user.Email,
            oldValues: new { Email = oldEmail, Roles = currentRoles.ToList() },
            newValues: new { Email = user.Email, Roles = roles.ToList() },
            additionalInfo: "User information and roles updated"
        );

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            UserName = user.UserName!,
            EmailConfirmed = user.EmailConfirmed,
            Roles = roles.ToList(),
            LockoutEnd = user.LockoutEnd?.DateTime,
            IsLockedOut = user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow,
            IsAccountSetupComplete = user.IsAccountSetupComplete,
            ScheduledDeletionDate = user.ScheduledDeletionDate
        };
    }

    public async Task<bool> UpdateUserPasswordAsync(string userId, UpdateUserPasswordDto passwordDto)
    {
        return false;
    }

    public async Task<bool> CompleteAccountSetupAsync(string email, string token, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || user.IsDeleted) return false;

        if (user.AccountSetupToken != token ||
            user.AccountSetupTokenExpiry == null ||
            user.AccountSetupTokenExpiry < DateTime.UtcNow)
        {
            return false;
        }

        var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, passwordResetToken, password);

        if (!result.Succeeded) return false;

        user.IsAccountSetupComplete = true;
        user.AccountSetupToken = null;
        user.AccountSetupTokenExpiry = null;
        await _userManager.UpdateAsync(user);

        return true;
    }

    public async Task<bool> DeleteUserAsync(string userId, string? currentUserId = null, string? currentUserEmail = null)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || user.IsDeleted) return false;

        var userRoles = await _userManager.GetRolesAsync(user);
        if (userRoles.Contains("Admin"))
        {
            var allUsers = await _userManager.Users.Where(u => !u.IsDeleted).ToListAsync();
            var adminCount = 0;
            foreach (var u in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(u);
                if (roles.Contains("Admin"))
                {
                    adminCount++;
                }
            }

            if (adminCount <= 1)
            {
                return false;
            }
        }

        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
        user.ScheduledDeletionDate = DateTime.UtcNow.AddDays(30);

        await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded) return false;

        await _emailService.SendAccountDeletionScheduledEmailAsync(user.Email!, user.ScheduledDeletionDate.Value);

        // Log audit event
        await _auditService.LogAsync(
            performedByUserId: currentUserId,
            performedByEmail: currentUserEmail,
            action: "DeleteUser",
            entity: "User",
            targetUserId: user.Id,
            targetEmail: user.Email,
            oldValues: new { IsDeleted = false, Roles = userRoles.ToList() },
            newValues: new { IsDeleted = true, ScheduledDeletionDate = user.ScheduledDeletionDate },
            additionalInfo: $"User scheduled for deletion on {user.ScheduledDeletionDate:yyyy-MM-dd}"
        );

        return true;
    }

    public async Task<bool> CancelDeletionAsync(string userId, string? currentUserId = null, string? currentUserEmail = null)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || !user.IsDeleted) return false;

        var previousDeletionDate = user.ScheduledDeletionDate;

        user.IsDeleted = false;
        user.DeletedAt = null;
        user.ScheduledDeletionDate = null;

        await _userManager.SetLockoutEndDateAsync(user, null);

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded) return false;

        await _emailService.SendAccountDeletionCancelledEmailAsync(user.Email!);

        // Log audit event
        await _auditService.LogAsync(
            performedByUserId: currentUserId,
            performedByEmail: currentUserEmail,
            action: "CancelDeletion",
            entity: "User",
            targetUserId: user.Id,
            targetEmail: user.Email,
            oldValues: new { IsDeleted = true, ScheduledDeletionDate = previousDeletionDate },
            newValues: new { IsDeleted = false, ScheduledDeletionDate = (DateTime?)null },
            additionalInfo: "User deletion cancelled and account restored"
        );

        return true;
    }

    public async Task<bool> ToggleUserLockoutAsync(string userId, string? currentUserId, string? currentUserEmail = null)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || user.IsDeleted) return false;

        if (userId == currentUserId)
        {
            return false;
        }

        bool wasLocked = user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow;

        if (wasLocked)
        {
            await _userManager.SetLockoutEndDateAsync(user, null);
            await _emailService.SendAccountUnlockedEmailAsync(user.Email!);

            // Log audit event
            await _auditService.LogAsync(
                performedByUserId: currentUserId,
                performedByEmail: currentUserEmail,
                action: "UnlockUser",
                entity: "User",
                targetUserId: user.Id,
                targetEmail: user.Email,
                oldValues: new { IsLockedOut = true },
                newValues: new { IsLockedOut = false },
                additionalInfo: "User account unlocked"
            );
        }
        else
        {
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
            await _emailService.SendAccountLockedEmailAsync(user.Email!);

            // Log audit event
            await _auditService.LogAsync(
                performedByUserId: currentUserId,
                performedByEmail: currentUserEmail,
                action: "LockUser",
                entity: "User",
                targetUserId: user.Id,
                targetEmail: user.Email,
                oldValues: new { IsLockedOut = false },
                newValues: new { IsLockedOut = true },
                additionalInfo: "User account locked"
            );
        }

        return true;
    }

    public async Task<List<RoleDto>> GetAllRolesAsync()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        var roleDtos = new List<RoleDto>();

        foreach (var role in roles)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
            var activeUsersCount = usersInRole.Count(u => !u.IsDeleted);

            roleDtos.Add(new RoleDto
            {
                Id = role.Id,
                Name = role.Name!,
                UserCount = activeUsersCount
            });
        }

        return roleDtos;
    }

    public async Task<bool> UpdateUserRolesAsync(string userId, UserRoleUpdateDto roleUpdateDto, string? currentUserId = null, string? currentUserEmail = null)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || user.IsDeleted) return false;

        var currentRoles = await _userManager.GetRolesAsync(user);

        // Validate that all roles exist before making changes
        if (roleUpdateDto.Roles.Any())
        {
            foreach (var roleName in roleUpdateDto.Roles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    return false; // Invalid role found
                }
            }
        }

        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        if (roleUpdateDto.Roles.Any())
        {
            var result = await _userManager.AddToRolesAsync(user, roleUpdateDto.Roles);

            if (result.Succeeded)
            {
                // Log audit event
                await _auditService.LogAsync(
                    performedByUserId: currentUserId,
                    performedByEmail: currentUserEmail,
                    action: "UpdateUserRoles",
                    entity: "User",
                    targetUserId: user.Id,
                    targetEmail: user.Email,
                    oldValues: new { Roles = currentRoles.ToList() },
                    newValues: new { Roles = roleUpdateDto.Roles },
                    additionalInfo: "User roles updated"
                );
            }

            return result.Succeeded;
        }

        // Log audit event when all roles removed
        await _auditService.LogAsync(
            performedByUserId: currentUserId,
            performedByEmail: currentUserEmail,
            action: "UpdateUserRoles",
            entity: "User",
            targetUserId: user.Id,
            targetEmail: user.Email,
            oldValues: new { Roles = currentRoles.ToList() },
            newValues: new { Roles = new List<string>() },
            additionalInfo: "All user roles removed"
        );

        return true;
    }

    public async Task PermanentlyDeleteExpiredAccountsAsync()
    {
        var now = DateTime.UtcNow;

        var softDeletedUsers = await _userManager.Users
            .Where(u => u.IsDeleted &&
                   u.ScheduledDeletionDate != null &&
                   u.ScheduledDeletionDate <= now)
            .ToListAsync();

        foreach (var user in softDeletedUsers)
        {
            await _userManager.DeleteAsync(user);
        }

        var expiredSetupAccounts = await _userManager.Users
            .Where(u => !u.IsAccountSetupComplete &&
                   u.AccountSetupTokenExpiry != null &&
                   u.AccountSetupTokenExpiry < now)
            .ToListAsync();

        foreach (var user in expiredSetupAccounts)
        {
            await _userManager.DeleteAsync(user);
        }
    }

    private string GenerateSecureToken()
    {
        var randomBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToBase64String(randomBytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');
    }
}
