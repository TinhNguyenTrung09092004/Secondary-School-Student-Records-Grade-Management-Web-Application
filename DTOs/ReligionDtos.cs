using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class ReligionDto
{
    public string ReligionId { get; set; } = null!;
    public string ReligionName { get; set; } = null!;
}

public class CreateReligionDto
{
    [Required(ErrorMessage = "Tên tôn giáo là bắt buộc")]
    [StringLength(30, ErrorMessage = "Tên tôn giáo không được vượt quá 30 ký tự")]
    public string ReligionName { get; set; } = null!;
}

public class UpdateReligionDto
{
    [Required(ErrorMessage = "Tên tôn giáo là bắt buộc")]
    [StringLength(30, ErrorMessage = "Tên tôn giáo không được vượt quá 30 ký tự")]
    public string ReligionName { get; set; } = null!;
}
