using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class SemesterDto
{
    public string SemesterId { get; set; } = null!;
    public string SemesterName { get; set; } = null!;
    public int Coefficient { get; set; }
}

public class CreateSemesterDto
{
    [Required(ErrorMessage = "Mã học kỳ là bắt buộc")]
    [StringLength(3, ErrorMessage = "Mã học kỳ không được vượt quá 3 ký tự")]
    public string SemesterId { get; set; } = null!;

    [Required(ErrorMessage = "Tên học kỳ là bắt buộc")]
    [StringLength(30, ErrorMessage = "Tên học kỳ không được vượt quá 30 ký tự")]
    public string SemesterName { get; set; } = null!;

    [Required(ErrorMessage = "Hệ số là bắt buộc")]
    [Range(1, int.MaxValue, ErrorMessage = "Hệ số phải lớn hơn 0")]
    public int Coefficient { get; set; }
}

public class UpdateSemesterDto
{
    [Required(ErrorMessage = "Mã học kỳ là bắt buộc")]
    [StringLength(3, ErrorMessage = "Mã học kỳ không được vượt quá 3 ký tự")]
    public string SemesterId { get; set; } = null!;

    [Required(ErrorMessage = "Tên học kỳ là bắt buộc")]
    [StringLength(30, ErrorMessage = "Tên học kỳ không được vượt quá 30 ký tự")]
    public string SemesterName { get; set; } = null!;

    [Required(ErrorMessage = "Hệ số là bắt buộc")]
    [Range(1, int.MaxValue, ErrorMessage = "Hệ số phải lớn hơn 0")]
    public int Coefficient { get; set; }
}
