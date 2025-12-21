using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Services;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UserManagementController : ControllerBase
{
    private readonly IUserManagementService _userManagementService;

    public UserManagementController(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    private string? GetCurrentUserId()
    {
        return User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
    }

    private string? GetCurrentUserEmail()
    {
        return User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userManagementService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("users/{userId}")]
    public async Task<IActionResult> GetUserById(string userId)
    {
        var user = await _userManagementService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return NotFound(new { message = "Người dùng không tồn tại" });
        }
        return Ok(user);
    }

    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        var currentUserId = GetCurrentUserId();
        var currentUserEmail = GetCurrentUserEmail();

        var user = await _userManagementService.CreateUserAsync(createUserDto, currentUserId, currentUserEmail);
        if (user == null)
        {
            return BadRequest(new { message = "Không thể tạo người dùng" });
        }
        return CreatedAtAction(nameof(GetUserById), new { userId = user.Id }, user);
    }

    [HttpPut("users/{userId}")]
    public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateUserDto updateUserDto)
    {
        var currentUserId = GetCurrentUserId();
        var currentUserEmail = GetCurrentUserEmail();

        var user = await _userManagementService.UpdateUserAsync(userId, updateUserDto, currentUserId, currentUserEmail);
        if (user == null)
        {
            return BadRequest(new { message = "Không thể cập nhật người dùng" });
        }
        return Ok(user);
    }

    [HttpPut("users/{userId}/password")]
    [Obsolete("This endpoint is deprecated. Users must set their own passwords via email link.")]
    public async Task<IActionResult> UpdateUserPassword(string userId, [FromBody] UpdateUserPasswordDto passwordDto)
    {
        return BadRequest(new { message = "Quản trị viên không thể đặt mật khẩu cho người dùng. Người dùng phải tự đặt mật khẩu qua email." });
    }

    [HttpDelete("users/{userId}")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var currentUserId = GetCurrentUserId();
        var currentUserEmail = GetCurrentUserEmail();

        // Prevent admin from deleting themselves
        if (userId == currentUserId)
        {
            return BadRequest(new { message = "Bạn không thể xóa chính mình" });
        }

        var result = await _userManagementService.DeleteUserAsync(userId, currentUserId, currentUserEmail);
        if (!result)
        {
            return BadRequest(new { message = "Không thể xóa người dùng. Hệ thống phải có ít nhất 1 Admin." });
        }
        return Ok(new { message = "Người dùng đã được lên lịch xóa. Tài khoản sẽ bị xóa vĩnh viễn sau 30 ngày." });
    }

    [HttpPost("users/{userId}/cancel-deletion")]
    public async Task<IActionResult> CancelDeletion(string userId)
    {
        var currentUserId = GetCurrentUserId();
        var currentUserEmail = GetCurrentUserEmail();

        var result = await _userManagementService.CancelDeletionAsync(userId, currentUserId, currentUserEmail);
        if (!result)
        {
            return BadRequest(new { message = "Không thể hủy xóa người dùng" });
        }
        return Ok(new { message = "Đã hủy xóa người dùng thành công" });
    }

    [HttpPost("users/{userId}/toggle-lockout")]
    public async Task<IActionResult> ToggleUserLockout(string userId)
    {
        var currentUserId = GetCurrentUserId();
        var currentUserEmail = GetCurrentUserEmail();

        var result = await _userManagementService.ToggleUserLockoutAsync(userId, currentUserId, currentUserEmail);
        if (!result)
        {
            return BadRequest(new { message = "Không thể thay đổi trạng thái khóa. Bạn không thể khóa tài khoản của chính mình." });
        }
        return Ok(new { message = "Trạng thái khóa đã được thay đổi" });
    }

    [HttpGet("roles")]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _userManagementService.GetAllRolesAsync();
        return Ok(roles);
    }

    [HttpPut("users/{userId}/roles")]
    public async Task<IActionResult> UpdateUserRoles(string userId, [FromBody] UserRoleUpdateDto roleUpdateDto)
    {
        var currentUserId = GetCurrentUserId();
        var currentUserEmail = GetCurrentUserEmail();

        var result = await _userManagementService.UpdateUserRolesAsync(userId, roleUpdateDto, currentUserId, currentUserEmail);
        if (!result)
        {
            return BadRequest(new { message = "Không thể cập nhật quyền" });
        }
        return Ok(new { message = "Quyền đã được cập nhật thành công" });
    }
}
