using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Models;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemSettingsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SystemSettingsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("{key}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSetting(string key)
    {
        var setting = await _context.SystemSettings.FindAsync(key);
        if (setting == null)
        {
            return NotFound(new { message = "Setting not found" });
        }
        return Ok(new { key = setting.Key, value = setting.Value });
    }

    [HttpGet("logo")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLogoUrl()
    {
        var setting = await _context.SystemSettings.FindAsync("LoginLogoUrl");
        if (setting == null)
        {
            return Ok(new { logoUrl = "https://res.cloudinary.com/dxfubqntx/image/upload/v1766312494/LOGO_k6u5iq.png" });
        }
        return Ok(new { logoUrl = setting.Value });
    }

    [HttpPost("logo")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateLogoUrl([FromBody] UpdateLogoRequest request)
    {
        var userId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

        var setting = await _context.SystemSettings.FindAsync("LoginLogoUrl");
        if (setting == null)
        {
            setting = new SystemSetting
            {
                Key = "LoginLogoUrl",
                Value = request.LogoUrl,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = userId
            };
            _context.SystemSettings.Add(setting);
        }
        else
        {
            setting.Value = request.LogoUrl;
            setting.UpdatedAt = DateTime.UtcNow;
            setting.UpdatedBy = userId;
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Logo updated successfully", logoUrl = setting.Value });
    }
}

public class UpdateLogoRequest
{
    public string LogoUrl { get; set; } = string.Empty;
}
