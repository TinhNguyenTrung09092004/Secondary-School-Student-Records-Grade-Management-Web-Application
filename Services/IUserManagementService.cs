using API.DTOs;

namespace API.Services;

public interface IUserManagementService
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(string userId);
    Task<UserDto?> CreateUserAsync(CreateUserDto createUserDto, string? currentUserId = null, string? currentUserEmail = null);
    Task<UserDto?> UpdateUserAsync(string userId, UpdateUserDto updateUserDto, string? currentUserId = null, string? currentUserEmail = null);
    Task<bool> UpdateUserPasswordAsync(string userId, UpdateUserPasswordDto passwordDto);
    Task<bool> CompleteAccountSetupAsync(string email, string token, string password);
    Task<bool> DeleteUserAsync(string userId, string? currentUserId = null, string? currentUserEmail = null);
    Task<bool> CancelDeletionAsync(string userId, string? currentUserId = null, string? currentUserEmail = null);
    Task<bool> ToggleUserLockoutAsync(string userId, string? currentUserId, string? currentUserEmail = null);
    Task<List<RoleDto>> GetAllRolesAsync();
    Task<bool> UpdateUserRolesAsync(string userId, UserRoleUpdateDto roleUpdateDto, string? currentUserId = null, string? currentUserEmail = null);
    Task PermanentlyDeleteExpiredAccountsAsync();
}
