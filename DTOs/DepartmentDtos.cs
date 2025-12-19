using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class CreateDepartmentDto
{
    [Required(ErrorMessage = "Mã tổ bộ môn là bắt buộc")]
    [StringLength(6, ErrorMessage = "Mã tổ bộ môn không được quá 6 ký tự")]
    public string DepartmentId { get; set; } = null!;

    [Required(ErrorMessage = "Tên tổ bộ môn là bắt buộc")]
    [StringLength(50, ErrorMessage = "Tên tổ bộ môn không được quá 50 ký tự")]
    public string DepartmentName { get; set; } = null!;

    [StringLength(6, ErrorMessage = "Mã giáo viên không được quá 6 ký tự")]
    public string? HeadTeacherId { get; set; }
}

public class UpdateDepartmentDto
{
    [Required(ErrorMessage = "Tên tổ bộ môn là bắt buộc")]
    [StringLength(50, ErrorMessage = "Tên tổ bộ môn không được quá 50 ký tự")]
    public string DepartmentName { get; set; } = null!;

    [StringLength(6, ErrorMessage = "Mã giáo viên không được quá 6 ký tự")]
    public string? HeadTeacherId { get; set; }
}

public class DepartmentResponseDto
{
    public string DepartmentId { get; set; } = null!;
    public string DepartmentName { get; set; } = null!;
    public string? HeadTeacherId { get; set; }
    public string? HeadTeacherName { get; set; }
    public int TeacherCount { get; set; }
}

public class DepartmentDetailDto
{
    public string DepartmentId { get; set; } = null!;
    public string DepartmentName { get; set; } = null!;
    public string? HeadTeacherId { get; set; }
    public string? HeadTeacherName { get; set; }
    public List<TeacherInDepartmentDto> Teachers { get; set; } = new();
}

public class TeacherInDepartmentDto
{
    public string TeacherId { get; set; } = null!;
    public string TeacherName { get; set; } = null!;
    public string SubjectName { get; set; } = null!;
}
