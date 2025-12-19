using Microsoft.AspNetCore.Identity;

namespace API.Models;

public class ApplicationUser : IdentityUser
{
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public DateTime? ScheduledDeletionDate { get; set; }
    public bool IsAccountSetupComplete { get; set; } = false;
    public string? AccountSetupToken { get; set; }
    public DateTime? AccountSetupTokenExpiry { get; set; }

    // Facial Recognition Fields
    public string? FaceDescriptor { get; set; } // JSON array of face descriptor (128-dimensional vector)
    public bool IsFaceEnrolled { get; set; } = false;
    public DateTime? FaceEnrolledAt { get; set; }
}
