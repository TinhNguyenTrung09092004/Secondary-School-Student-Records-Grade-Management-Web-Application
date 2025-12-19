using API.DTOs;

namespace API.Services;

public interface IFaceRecognitionService
{
    Task<bool> EnrollFaceAsync(EnrollFaceDto enrollDto);
    Task<LoginResponseDto?> AuthenticateWithFaceAsync(FaceLoginDto faceLoginDto);
    Task<FaceEnrollmentStatusDto> GetEnrollmentStatusAsync(string email);
}