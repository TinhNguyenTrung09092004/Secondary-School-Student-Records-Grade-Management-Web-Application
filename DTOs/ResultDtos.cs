using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class ResultDto
{
    public string ResultId { get; set; } = null!;
    public string ResultName { get; set; } = null!;
}

public class CreateResultDto
{
    [Required(ErrorMessage = "Mã kết quả là bắt buộc")]
    [StringLength(6, ErrorMessage = "Mã kết quả không được vượt quá 6 ký tự")]
    public string ResultId { get; set; } = null!;

    [Required(ErrorMessage = "Tên kết quả là bắt buộc")]
    [StringLength(30, ErrorMessage = "Tên kết quả không được vượt quá 30 ký tự")]
    public string ResultName { get; set; } = null!;
}

public class UpdateResultDto
{
    [Required(ErrorMessage = "Tên kết quả là bắt buộc")]
    [StringLength(30, ErrorMessage = "Tên kết quả không được vượt quá 30 ký tự")]
    public string ResultName { get; set; } = null!;
}
