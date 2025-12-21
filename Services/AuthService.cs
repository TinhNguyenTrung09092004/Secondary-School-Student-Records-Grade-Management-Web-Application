using API.DTOs;
using API.Helpers;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Google.Apis.Auth;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtHelper _jwtHelper;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public AuthService(UserManager<ApplicationUser> userManager, JwtHelper jwtHelper, IConfiguration configuration, IEmailService emailService)
    {
        _userManager = userManager;
        _jwtHelper = jwtHelper;
        _configuration = configuration;
        _emailService = emailService;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return null;
        }

        if (user.IsDeleted)
        {
            throw new Exception("Account has been deleted");
        }

        if (!user.IsAccountSetupComplete)
        {
            throw new Exception("Account setup is not complete. Please check your email for the setup link.");
        }

        if (await _userManager.IsLockedOutAsync(user))
        {
            throw new Exception("Account is locked. Please contact an administrator.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtHelper.GenerateToken(user, roles);

        return new LoginResponseDto
        {
            Token = token,
            Email = user.Email!,
            Roles = roles.ToList()
        };
    }

    public async Task<LoginResponseDto?> ExternalLoginAsync(ExternalAuthDto externalAuth)
    {
        string? email = null;

        try
        {
            if (externalAuth.Provider.ToLower() == "google")
            {
                var googleClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");

                if (string.IsNullOrEmpty(googleClientId))
                {
                    throw new Exception("GOOGLE_CLIENT_ID environment variable not set");
                }

                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { googleClientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(externalAuth.IdToken, settings);
                email = payload.Email;
            }
            else if (externalAuth.Provider.ToLower() == "microsoft")
            {
                var microsoftClientId = Environment.GetEnvironmentVariable("MICROSOFT_CLIENT_ID");

                if (string.IsNullOrEmpty(microsoftClientId))
                {
                    throw new Exception("MICROSOFT_CLIENT_ID environment variable not set");
                }

                var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                    "https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration",
                    new OpenIdConnectConfigurationRetriever(),
                    new HttpDocumentRetriever());

                var discoveryDocument = await configurationManager.GetConfigurationAsync();
                var signingKeys = discoveryDocument.SigningKeys;

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    IssuerValidator = (issuer, token, parameters) =>
                    {
                        if (issuer.StartsWith("https://login.microsoftonline.com/") && issuer.EndsWith("/v2.0"))
                        {
                            return issuer;
                        }
                        throw new SecurityTokenInvalidIssuerException($"Invalid issuer: {issuer}");
                    },
                    ValidateAudience = true,
                    ValidAudience = microsoftClientId,
                    ValidateLifetime = true,
                    IssuerSigningKeys = signingKeys,
                    ValidateIssuerSigningKey = true
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(externalAuth.IdToken, validationParameters, out var validatedToken);

                email = principal.FindFirst("preferred_username")?.Value
                    ?? principal.FindFirst("email")?.Value
                    ?? principal.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(email))
                {
                    throw new Exception("Email not found in Microsoft token");
                }
            }

            if (string.IsNullOrEmpty(email))
            {
                throw new Exception("Email not found in OAuth token");
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                throw new Exception("Account not found. Please contact an administrator to create your account.");
            }

            if (user.IsDeleted)
            {
                throw new Exception("Account has been deleted");
            }

            if (!user.IsAccountSetupComplete)
            {
                throw new Exception("Account setup is not complete. Please check your email for the setup link.");
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                throw new Exception("Account is locked. Please contact an administrator.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtHelper.GenerateToken(user, roles);

            return new LoginResponseDto
            {
                Token = token,
                Email = user.Email!,
                Roles = roles.ToList()
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"External login failed for {externalAuth.Provider}: {ex.Message}", ex);
        }
    }

    public string GetExternalLoginUrl(string provider, string redirectUri)
    {
        return $"/api/auth/external-login?provider={provider}&redirectUri={redirectUri}";
    }

    public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto request, string resetUrl)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return true;
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = System.Web.HttpUtility.UrlEncode(token);
        var encodedEmail = System.Web.HttpUtility.UrlEncode(request.Email);
        var resetLink = $"{resetUrl}?email={encodedEmail}&token={encodedToken}&type=reset";

        await _emailService.SendPasswordResetEmailAsync(request.Email, resetLink);

        return true;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return false;
        }

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        return result.Succeeded;
    }

    public async Task<bool> ChangeOwnPasswordAsync(string userId, ChangeOwnPasswordDto request)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null || user.IsDeleted)
        {
            return false;
        }

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        return result.Succeeded;
    }
}
