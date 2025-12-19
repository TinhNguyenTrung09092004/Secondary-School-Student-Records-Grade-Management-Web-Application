using API.DTOs;
using API.Helpers;
using API.Models;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;

namespace API.Services;

public class FaceRecognitionService : IFaceRecognitionService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtHelper _jwtHelper;
    private const double FACE_MATCH_THRESHOLD = 0.5; // Standard threshold for face-api.js (Euclidean distance)

    public FaceRecognitionService(UserManager<ApplicationUser> userManager, JwtHelper jwtHelper)
    {
        _userManager = userManager;
        _jwtHelper = jwtHelper;
    }

    public async Task<bool> EnrollFaceAsync(EnrollFaceDto enrollDto)
    {
        var user = await _userManager.FindByEmailAsync(enrollDto.Email);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        if (user.IsDeleted)
        {
            throw new Exception("Account has been deleted");
        }

        if (!user.IsAccountSetupComplete)
        {
            throw new Exception("Account setup is not complete");
        }

        // Validate face descriptor
        if (enrollDto.FaceDescriptor == null || enrollDto.FaceDescriptor.Length != 128)
        {
            throw new Exception("Invalid face descriptor. Expected 128-dimensional vector.");
        }

        // Store the face descriptor as JSON
        user.FaceDescriptor = JsonSerializer.Serialize(enrollDto.FaceDescriptor);
        user.IsFaceEnrolled = true;
        user.FaceEnrolledAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<LoginResponseDto?> AuthenticateWithFaceAsync(FaceLoginDto faceLoginDto)
    {
        // Validate face descriptor
        if (faceLoginDto.FaceDescriptor == null || faceLoginDto.FaceDescriptor.Length != 128)
        {
            throw new Exception("Invalid face descriptor. Expected 128-dimensional vector.");
        }

        // Get all users with enrolled faces
        var allUsers = _userManager.Users.Where(u => u.IsFaceEnrolled && !u.IsDeleted).ToList();

        ApplicationUser? matchedUser = null;
        double bestDistance = double.MaxValue;

        // Find the best matching face
        foreach (var user in allUsers)
        {
            if (string.IsNullOrEmpty(user.FaceDescriptor))
                continue;

            try
            {
                var storedDescriptor = JsonSerializer.Deserialize<double[]>(user.FaceDescriptor);
                if (storedDescriptor == null || storedDescriptor.Length != 128)
                    continue;

                // Calculate Euclidean distance
                double distance = CalculateEuclideanDistance(faceLoginDto.FaceDescriptor, storedDescriptor);

                if (distance < bestDistance && distance < FACE_MATCH_THRESHOLD)
                {
                    bestDistance = distance;
                    matchedUser = user;
                }
            }
            catch
            {
                continue;
            }
        }

        if (matchedUser == null)
        {
            return null; // No match found
        }

        // Check account status
        if (!matchedUser.IsAccountSetupComplete)
        {
            throw new Exception("Account setup is not complete");
        }

        if (await _userManager.IsLockedOutAsync(matchedUser))
        {
            throw new Exception("Account is locked. Please contact an administrator.");
        }

        // Generate JWT token
        var roles = await _userManager.GetRolesAsync(matchedUser);
        var token = _jwtHelper.GenerateToken(matchedUser, roles);

        return new LoginResponseDto
        {
            Token = token,
            Email = matchedUser.Email!,
            Roles = roles.ToList()
        };
    }

    public async Task<FaceEnrollmentStatusDto> GetEnrollmentStatusAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            throw new Exception("User not found");
        }

        return new FaceEnrollmentStatusDto
        {
            IsEnrolled = user.IsFaceEnrolled,
            EnrolledAt = user.FaceEnrolledAt
        };
    }

    private double CalculateEuclideanDistance(double[] descriptor1, double[] descriptor2)
    {
        if (descriptor1.Length != descriptor2.Length)
        {
            throw new ArgumentException("Descriptors must have the same length");
        }

        double sum = 0;
        for (int i = 0; i < descriptor1.Length; i++)
        {
            double diff = descriptor1[i] - descriptor2[i];
            sum += diff * diff;
        }

        return Math.Sqrt(sum);
    }
}