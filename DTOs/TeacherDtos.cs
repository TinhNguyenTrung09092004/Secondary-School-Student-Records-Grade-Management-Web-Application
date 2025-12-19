using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class TeacherDto
{
    public string TeacherId { get; set; } = null!;
    public string? UserId { get; set; }
    public string TeacherName { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string SubjectId { get; set; } = null!;
    public string? DepartmentId { get; set; }
}

public class CreateTeacherProfileDto
{
    [Required(ErrorMessage = "Mã giáo viên là bắt buộc")]
    [StringLength(6, ErrorMessage = "Mã giáo viên không được vượt quá 6 ký tự")]
    public string TeacherId { get; set; } = null!;

    [Required(ErrorMessage = "Tên giáo viên là bắt buộc")]
    [StringLength(30, ErrorMessage = "Tên giáo viên không được vượt quá 30 ký tự")]
    public string TeacherName { get; set; } = null!;

    [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
    [StringLength(50, ErrorMessage = "Địa chỉ không được vượt quá 50 ký tự")]
    public string Address { get; set; } = null!;

    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string PhoneNumber { get; set; } = null!;

    [Required(ErrorMessage = "Môn học là bắt buộc")]
    [StringLength(6)]
    public string SubjectId { get; set; } = null!;

    [StringLength(6)]
    public string? DepartmentId { get; set; }
}

public class UpdateTeacherProfileDto
{
    [Required(ErrorMessage = "Tên giáo viên là bắt buộc")]
    [StringLength(30, ErrorMessage = "Tên giáo viên không được vượt quá 30 ký tự")]
    public string TeacherName { get; set; } = null!;

    [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
    [StringLength(50, ErrorMessage = "Địa chỉ không được vượt quá 50 ký tự")]
    public string Address { get; set; } = null!;

    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string PhoneNumber { get; set; } = null!;

    [Required(ErrorMessage = "Môn học là bắt buộc")]
    [StringLength(6)]
    public string SubjectId { get; set; } = null!;

    [StringLength(6)]
    public string? DepartmentId { get; set; }
}
