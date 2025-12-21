namespace API.DTOs;

public class ChangeOwnPasswordDto
{
    public string CurrentPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}
