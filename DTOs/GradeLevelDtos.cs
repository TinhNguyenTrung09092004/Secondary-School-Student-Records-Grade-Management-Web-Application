using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class GradeLevelDto
{
    public string GradeLevelId { get; set; } = null!;
    public string GradeLevelName { get; set; } = null!;
}

public class CreateGradeLevelDto
{
    [Required(ErrorMessage = "Mã khối là bắt buộc")]
    [StringLength(6, ErrorMessage = "Mã khối không được vượt quá 6 ký tự")]
    public string GradeLevelId { get; set; } = null!;

    [Required(ErrorMessage = "Tên khối là bắt buộc")]
    [StringLength(30, ErrorMessage = "Tên khối không được vượt quá 30 ký tự")]
    public string GradeLevelName { get; set; } = null!;
}

public class UpdateGradeLevelDto
{
    [Required(ErrorMessage = "Mã khối là bắt buộc")]
    [StringLength(6, ErrorMessage = "Mã khối không được vượt quá 6 ký tự")]
    public string GradeLevelId { get; set; } = null!;

    [Required(ErrorMessage = "Tên khối là bắt buộc")]
    [StringLength(30, ErrorMessage = "Tên khối không được vượt quá 30 ký tự")]
    public string GradeLevelName { get; set; } = null!;
}
