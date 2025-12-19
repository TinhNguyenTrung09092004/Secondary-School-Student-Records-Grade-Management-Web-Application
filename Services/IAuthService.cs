using API.DTOs;

namespace API.Services;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
    Task<LoginResponseDto?> ExternalLoginAsync(ExternalAuthDto externalAuth);
    string GetExternalLoginUrl(string provider, string redirectUri);
    Task<bool> ForgotPasswordAsync(ForgotPasswordDto request, string resetUrl);
    Task<bool> ResetPasswordAsync(ResetPasswordDto request);
}
