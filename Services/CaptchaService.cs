using System.Collections.Concurrent;

namespace API.Services;

public class CaptchaService : ICaptchaService
{
    private readonly ConcurrentDictionary<string, int> _failedAttempts = new();
    private readonly ConcurrentDictionary<string, (int answer, DateTime expiry)> _captchas = new();
    private readonly Random _random = new();

    public (string question, string token) GenerateCaptcha()
    {
        // Generate 6-character alphanumeric code
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // Exclude confusing chars like 0, O, 1, I
        var code = new string(Enumerable.Range(0, 6)
            .Select(_ => chars[_random.Next(chars.Length)])
            .ToArray());

        var token = Guid.NewGuid().ToString();

        // Store the code as a string in the tuple (convert to int for compatibility)
        _captchas[token] = (code.GetHashCode(), DateTime.UtcNow.AddMinutes(5));

        // Store the actual code separately
        _captchaAnswers[token] = code;

        return (code, token);
    }

    private readonly ConcurrentDictionary<string, string> _captchaAnswers = new();

    public bool VerifyCaptcha(string token, string answer)
    {
        if (!_captchaAnswers.TryGetValue(token, out var correctAnswer))
            return false;

        if (!_captchas.TryGetValue(token, out var captcha))
            return false;

        if (captcha.expiry < DateTime.UtcNow)
        {
            _captchas.TryRemove(token, out _);
            _captchaAnswers.TryRemove(token, out _);
            return false;
        }

        var isValid = string.Equals(answer?.Trim(), correctAnswer, StringComparison.OrdinalIgnoreCase);

        if (isValid)
        {
            _captchas.TryRemove(token, out _);
            _captchaAnswers.TryRemove(token, out _);
        }

        return isValid;
    }

    public void RecordFailedAttempt(string email)
    {
        _failedAttempts.AddOrUpdate(email.ToLower(), 1, (key, count) => count + 1);
    }

    public bool RequiresCaptcha(string email)
    {
        return _failedAttempts.TryGetValue(email.ToLower(), out var count) && count >= 2;
    }

    public void ResetAttempts(string email)
    {
        _failedAttempts.TryRemove(email.ToLower(), out _);
    }
}
