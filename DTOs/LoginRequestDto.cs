namespace API.DTOs;

public class LoginRequestDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? CaptchaToken { get; set; }
    public string? CaptchaAnswer { get; set; }
}
