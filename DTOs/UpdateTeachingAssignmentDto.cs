using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class UpdateTeachingAssignmentDto
{
    [Required(ErrorMessage = "Năm học là bắt buộc")]
    [StringLength(6, ErrorMessage = "Mã năm học không được quá 6 ký tự")]
    public string SchoolYearId { get; set; } = null!;

    [Required(ErrorMessage = "Lớp học là bắt buộc")]
    [StringLength(10, ErrorMessage = "Mã lớp học không được quá 10 ký tự")]
    public string ClassId { get; set; } = null!;

    [Required(ErrorMessage = "Môn học là bắt buộc")]
    [StringLength(6, ErrorMessage = "Mã môn học không được quá 6 ký tự")]
    public string SubjectId { get; set; } = null!;

    [Required(ErrorMessage = "Giáo viên là bắt buộc")]
    [StringLength(6, ErrorMessage = "Mã giáo viên không được quá 6 ký tự")]
    public string TeacherId { get; set; } = null!;
}
