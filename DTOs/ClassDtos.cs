using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class ClassDto
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

public class CreateClassDto
{
    [Required]
    [StringLength(10)]
    public string ClassId { get; set; } = null!;

    [Required]
    [StringLength(30)]
    public string ClassName { get; set; } = null!;

    [Required]
    [StringLength(6)]
    public string GradeLevelId { get; set; } = null!;

    [Required]
    [StringLength(6)]
    public string SchoolYearId { get; set; } = null!;

    [Required]
    [Range(1, 100, ErrorMessage = "Sĩ số phải từ 1 đến 100")]
    public int ClassSize { get; set; }
}

public class UpdateClassDto
{
    [Required]
    [StringLength(10)]
    public string ClassId { get; set; } = null!;

    [Required]
    [StringLength(30)]
    public string ClassName { get; set; } = null!;

    [Required]
    [StringLength(6)]
    public string GradeLevelId { get; set; } = null!;

    [Required]
    [StringLength(6)]
    public string SchoolYearId { get; set; } = null!;

    [Required]
    [Range(1, 100, ErrorMessage = "Sĩ số phải từ 1 đến 100")]
    public int ClassSize { get; set; }
}

public class AssignTeacherDto
{
    [Required]
    [StringLength(6)]
    public string TeacherId { get; set; } = null!;
}
