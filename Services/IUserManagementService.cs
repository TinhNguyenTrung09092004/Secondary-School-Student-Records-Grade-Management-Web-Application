using API.DTOs;

namespace API.Services;

public interface IUserManagementService
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(string userId);
    Task<UserDto?> CreateUserAsync(CreateUserDto createUserDto);
    Task<UserDto?> UpdateUserAsync(string userId, UpdateUserDto updateUserDto);
    Task<bool> UpdateUserPasswordAsync(string userId, UpdateUserPasswordDto passwordDto);
    Task<bool> CompleteAccountSetupAsync(string email, string token, string password);
    Task<bool> DeleteUserAsync(string userId);
    Task<bool> CancelDeletionAsync(string userId);
    Task<bool> ToggleUserLockoutAsync(string userId, string? currentUserId);
    Task<List<RoleDto>> GetAllRolesAsync();
    Task<bool> UpdateUserRolesAsync(string userId, UserRoleUpdateDto roleUpdateDto);
    Task PermanentlyDeleteExpiredAccountsAsync();
}
