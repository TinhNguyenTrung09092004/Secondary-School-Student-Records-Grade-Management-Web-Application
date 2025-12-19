using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class StudentConductDto
{
    public string StudentId { get; set; } = null!;
    public string StudentName { get; set; } = null!;
    public string ClassId { get; set; } = null!;
    public string ClassName { get; set; } = null!;
    public string SchoolYearId { get; set; } = null!;
    public string? Semester1ConductId { get; set; }
    public string? Semester1ConductName { get; set; }
    public string? Semester2ConductId { get; set; }
    public string? Semester2ConductName { get; set; }
    public string? YearEndConductId { get; set; }
    public string? YearEndConductName { get; set; }
}

public class UpdateStudentConductDto
{
    [Required(ErrorMessage = "Mã học sinh là bắt buộc")]
    public string StudentId { get; set; } = null!;

    [Required(ErrorMessage = "Mã lớp là bắt buộc")]
    public string ClassId { get; set; } = null!;

    [Required(ErrorMessage = "Mã năm học là bắt buộc")]
    public string SchoolYearId { get; set; } = null!;

    [Required(ErrorMessage = "Mã học kỳ là bắt buộc")]
    public string SemesterId { get; set; } = null!;

    [Required(ErrorMessage = "Mã hạnh kiểm là bắt buộc")]
    public string ConductId { get; set; } = null!;
}

public class ClassForConductGradingDto
{
    public string ClassId { get; set; } = null!;
    public string ClassName { get; set; } = null!;
    public string SchoolYearId { get; set; } = null!;
    public string SchoolYearName { get; set; } = null!;
    public int StudentCount { get; set; }
}
