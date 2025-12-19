using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class OccupationDto
{
    public string OccupationId { get; set; } = null!;
    public string OccupationName { get; set; } = null!;
}

public class CreateOccupationDto
{
    [Required(ErrorMessage = "Tên nghề nghiệp là bắt buộc")]
    [StringLength(30, ErrorMessage = "Tên nghề nghiệp không được vượt quá 30 ký tự")]
    public string OccupationName { get; set; } = null!;
}

public class UpdateOccupationDto
{
    [Required(ErrorMessage = "Tên nghề nghiệp là bắt buộc")]
    [StringLength(30, ErrorMessage = "Tên nghề nghiệp không được vượt quá 30 ký tự")]
    public string OccupationName { get; set; } = null!;
}
