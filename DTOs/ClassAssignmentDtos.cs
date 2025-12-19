using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class ClassAssignmentDto
{
    public string SchoolYearId { get; set; } = null!;
    public string SchoolYearName { get; set; } = null!;
    public string GradeLevelId { get; set; } = null!;
    public string GradeLevelName { get; set; } = null!;
    public string ClassId { get; set; } = null!;
    public string ClassName { get; set; } = null!;
    public string StudentId { get; set; } = null!;
    public string StudentName { get; set; } = null!;
}

public class StudentInClassDto
{
    public string StudentId { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool Gender { get; set; }
}

public class ClassInfoDto
{
    public string ClassId { get; set; } = null!;
    public string ClassName { get; set; } = null!;
    public string GradeLevelId { get; set; } = null!;
    public string GradeLevelName { get; set; } = null!;
    public string SchoolYearId { get; set; } = null!;
    public string SchoolYearName { get; set; } = null!;
    public int ClassSize { get; set; }
    public int CurrentStudentCount { get; set; }
    public string? TeacherId { get; set; }
    public string? TeacherName { get; set; }
}

public class AssignStudentToClassDto
{
    [Required(ErrorMessage = "Mã năm học là bắt buộc")]
    public string SchoolYearId { get; set; } = null!;

    [Required(ErrorMessage = "Mã khối là bắt buộc")]
    public string GradeLevelId { get; set; } = null!;

    [Required(ErrorMessage = "Mã lớp là bắt buộc")]
    public string ClassId { get; set; } = null!;

    [Required(ErrorMessage = "Mã học sinh là bắt buộc")]
    public string StudentId { get; set; } = null!;
}

public class BulkAssignStudentsDto
{
    [Required(ErrorMessage = "Mã năm học là bắt buộc")]
    public string SchoolYearId { get; set; } = null!;

    [Required(ErrorMessage = "Mã khối là bắt buộc")]
    public string GradeLevelId { get; set; } = null!;

    [Required(ErrorMessage = "Mã lớp là bắt buộc")]
    public string ClassId { get; set; } = null!;

    [Required(ErrorMessage = "Danh sách học sinh là bắt buộc")]
    [MinLength(1, ErrorMessage = "Phải có ít nhất 1 học sinh")]
    public List<string> StudentIds { get; set; } = new List<string>();
}

public class RemoveStudentFromClassDto
{
    [Required(ErrorMessage = "Mã năm học là bắt buộc")]
    public string SchoolYearId { get; set; } = null!;

    [Required(ErrorMessage = "Mã khối là bắt buộc")]
    public string GradeLevelId { get; set; } = null!;

    [Required(ErrorMessage = "Mã lớp là bắt buộc")]
    public string ClassId { get; set; } = null!;

    [Required(ErrorMessage = "Mã học sinh là bắt buộc")]
    public string StudentId { get; set; } = null!;
}

public class AvailableStudentsQuery
{
    public string SchoolYearId { get; set; } = null!;
    public string GradeLevelId { get; set; } = null!;
}
