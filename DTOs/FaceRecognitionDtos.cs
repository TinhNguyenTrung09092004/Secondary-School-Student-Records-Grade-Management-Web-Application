namespace API.DTOs;

public class EnrollFaceDto
{
    public required string Email { get; set; }
    public required double[] FaceDescriptor { get; set; } // 128-dimensional face descriptor from face-api.js
}

public class FaceLoginDto
{
    public required double[] FaceDescriptor { get; set; }
}

public class FaceEnrollmentStatusDto
{
    public bool IsEnrolled { get; set; }
    public DateTime? EnrolledAt { get; set; }
}