namespace API.DTOs;

public class UserDto
{
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public bool EmailConfirmed { get; set; }
    public List<string> Roles { get; set; } = new();
    public DateTime? LockoutEnd { get; set; }
    public bool IsLockedOut { get; set; }
    public bool IsAccountSetupComplete { get; set; }
    public DateTime? ScheduledDeletionDate { get; set; }
}

public class CreateUserDto
{
    public string Email { get; set; } = null!;
    // Password removed - users set their own password via email link
    public List<string> Roles { get; set; } = new();
}

public class CompleteAccountSetupDto
{
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class UpdateUserDto
{
    public string Email { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
}

public class UpdateUserPasswordDto
{
    public string NewPassword { get; set; } = null!;
}

public class RoleDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int UserCount { get; set; }
}

public class UserRoleUpdateDto
{
    public List<string> Roles { get; set; } = new();
}
