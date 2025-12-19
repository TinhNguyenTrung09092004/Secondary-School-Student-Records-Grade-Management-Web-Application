using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class SchoolYearDto
{
    public string SchoolYearId { get; set; } = null!;
    public string SchoolYearName { get; set; } = null!;
}

public class CreateSchoolYearDto
{
    [Required(ErrorMessage = "Mã năm học là bắt buộc")]
    [StringLength(6, ErrorMessage = "Mã năm học không được vượt quá 6 ký tự")]
    public string SchoolYearId { get; set; } = null!;

    [Required(ErrorMessage = "Tên năm học là bắt buộc")]
    [StringLength(30, ErrorMessage = "Tên năm học không được vượt quá 30 ký tự")]
    public string SchoolYearName { get; set; } = null!;
}

public class UpdateSchoolYearDto
{
    [Required(ErrorMessage = "Mã năm học là bắt buộc")]
    [StringLength(6, ErrorMessage = "Mã năm học không được vượt quá 6 ký tự")]
    public string SchoolYearId { get; set; } = null!;

    [Required(ErrorMessage = "Tên năm học là bắt buộc")]
    [StringLength(30, ErrorMessage = "Tên năm học không được vượt quá 30 ký tự")]
    public string SchoolYearName { get; set; } = null!;
}
