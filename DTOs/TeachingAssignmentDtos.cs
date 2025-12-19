using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class TeachingAssignmentDto
{
    public int RowNumber { get; set; }
    public string SchoolYearId { get; set; } = null!;
    public string SchoolYearName { get; set; } = null!;
    public string ClassId { get; set; } = null!;
    public string ClassName { get; set; } = null!;
    public string SubjectId { get; set; } = null!;
    public string SubjectName { get; set; } = null!;
    public string TeacherId { get; set; } = null!;
    public string TeacherName { get; set; } = null!;
}

public class CreateTeachingAssignmentDto
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

public class DepartmentTeacherDto
{
    public string TeacherId { get; set; } = null!;
    public string TeacherName { get; set; } = null!;
    public string SubjectId { get; set; } = null!;
    public string SubjectName { get; set; } = null!;
    public bool IsHeadTeacher { get; set; }
}
