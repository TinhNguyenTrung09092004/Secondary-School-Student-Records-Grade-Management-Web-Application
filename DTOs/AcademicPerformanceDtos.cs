using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class AcademicPerformanceDto
{
    public string AcademicPerformanceId { get; set; } = null!;
    public string AcademicPerformanceName { get; set; } = null!;
}

public class CreateAcademicPerformanceDto
{
    [Required(ErrorMessage = "Mã học lực là bắt buộc")]
    [StringLength(6, ErrorMessage = "Mã học lực không được vượt quá 6 ký tự")]
    public string AcademicPerformanceId { get; set; } = null!;

    [Required(ErrorMessage = "Tên học lực là bắt buộc")]
    [StringLength(30, ErrorMessage = "Tên học lực không được vượt quá 30 ký tự")]
    public string AcademicPerformanceName { get; set; } = null!;
}

public class UpdateAcademicPerformanceDto
{
    [Required(ErrorMessage = "Tên học lực là bắt buộc")]
    [StringLength(30, ErrorMessage = "Tên học lực không được vượt quá 30 ký tự")]
    public string AcademicPerformanceName { get; set; } = null!;
}
