using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Services;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserManagementService _userManagementService;

    public AuthController(IAuthService authService, IUserManagementService userManagementService)
    {
        _authService = authService;
        _userManagementService = userManagementService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);

        if (result == null)
        {
            return Unauthorized(new { message = "Email hoặc mật khẩu không đúng" });
        }

        return Ok(result);
    }

    [HttpPost("external-login")]
    public async Task<IActionResult> ExternalLogin([FromBody] ExternalAuthDto externalAuth)
    {
        try
        {
            var result = await _authService.ExternalLoginAsync(externalAuth);

            if (result == null)
            {
                return Unauthorized(new { message = "Đăng nhập thất bại" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new {
                message = "Xác thực thất bại",
                error = ex.Message,
                details = ex.InnerException?.Message
            });
        }
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
    {
        try
        {
            // Use frontend URL from environment variable or default to localhost:4200
            var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL") ?? "http://localhost:4200";
            var resetUrl = $"{frontendUrl}/validate-token";
            var result = await _authService.ForgotPasswordAsync(request, resetUrl);

            return Ok(new { message = "Nếu email tồn tại, liên kết đặt lại mật khẩu đã được gửi" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Forgot Password Error: {ex.Message}");
            return StatusCode(500, new { message = "Có lỗi xảy ra" });
        }
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
    {
        try
        {
            var result = await _authService.ResetPasswordAsync(request);

            if (!result)
            {
                return BadRequest(new { message = "Không thể đặt lại mật khẩu. Liên kết có thể đã hết hạn" });
            }

            return Ok(new { message = "Mật khẩu đã được đặt lại thành công" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Có lỗi xảy ra" });
        }
    }

    [HttpPost("complete-account-setup")]
    public async Task<IActionResult> CompleteAccountSetup([FromBody] CompleteAccountSetupDto request)
    {
        try
        {
            var result = await _userManagementService.CompleteAccountSetupAsync(request.Email, request.Token, request.Password);

            if (!result)
            {
                return BadRequest(new { message = "Liên kết thiết lập tài khoản không hợp lệ hoặc đã hết hạn" });
            }

            return Ok(new { message = "Tài khoản đã được thiết lập thành công. Bạn có thể đăng nhập ngay bây giờ." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Có lỗi xảy ra" });
        }
    }
}