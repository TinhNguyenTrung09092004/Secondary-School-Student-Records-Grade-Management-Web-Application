namespace API.Services;

public interface ICaptchaService
{
    (string question, string token) GenerateCaptcha();
    bool VerifyCaptcha(string token, string answer);
    void RecordFailedAttempt(string email);
    bool RequiresCaptcha(string email);
    void ResetAttempts(string email);
}
