using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Models;
using API.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers;

public class JwtHelper
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;

    public JwtHelper(IConfiguration configuration, ApplicationDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    public string GenerateToken(ApplicationUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Add TeacherId if user is a teacher
        var teacher = _context.Teachers.FirstOrDefault(t => t.UserId == user.Id);
        if (teacher != null)
        {
            claims.Add(new Claim("TeacherId", teacher.TeacherId));
        }

        // Add StudentId if user is a student
        var student = _context.Students.FirstOrDefault(s => s.UserId == user.Id);
        if (student != null)
        {
            claims.Add(new Claim("StudentId", student.StudentId));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            Environment.GetEnvironmentVariable("JWT_KEY")!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: Environment.GetEnvironmentVariable("JWT_ISSUER"),
            audience: Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
