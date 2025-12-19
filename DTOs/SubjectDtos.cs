using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class SubjectDto
{
    public string SubjectId { get; set; } = null!;
    public string SubjectName { get; set; } = null!;
    public int LessonCount { get; set; }
    public int Coefficient { get; set; }
}

public class CreateSubjectDto
{
    [Required(ErrorMessage = "Tên môn học là bắt buộc")]
    [StringLength(30, ErrorMessage = "Tên môn học không được vượt quá 30 ký tự")]
    public string SubjectName { get; set; } = null!;

    [Required(ErrorMessage = "Số tiết học là bắt buộc")]
    [Range(1, int.MaxValue, ErrorMessage = "Số tiết học phải lớn hơn 0")]
    public int LessonCount { get; set; }

    [Required(ErrorMessage = "Hệ số là bắt buộc")]
    [Range(1, int.MaxValue, ErrorMessage = "Hệ số phải lớn hơn 0")]
    public int Coefficient { get; set; }
}

public class UpdateSubjectDto
{
    [Required(ErrorMessage = "Tên môn học là bắt buộc")]
    [StringLength(30, ErrorMessage = "Tên môn học không được vượt quá 30 ký tự")]
    public string SubjectName { get; set; } = null!;

    [Required(ErrorMessage = "Số tiết học là bắt buộc")]
    [Range(1, int.MaxValue, ErrorMessage = "Số tiết học phải lớn hơn 0")]
    public int LessonCount { get; set; }

    [Required(ErrorMessage = "Hệ số là bắt buộc")]
    [Range(1, int.MaxValue, ErrorMessage = "Hệ số phải lớn hơn 0")]
    public int Coefficient { get; set; }
}
