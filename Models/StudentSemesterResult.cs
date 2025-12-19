using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

[Table("STUDENT_SEMESTER_RESULT")]
public class StudentSemesterResult
{
    [Column("StudentId")]
    [StringLength(6)]
    public string StudentId { get; set; } = null!;

    [Column("ClassId")]
    [StringLength(10)]
    public string ClassId { get; set; } = null!;

    [Column("SchoolYearId")]
    [StringLength(6)]
    public string SchoolYearId { get; set; } = null!;

    [Column("SemesterId")]
    [StringLength(3)]
    public string SemesterId { get; set; } = null!;

    [Column("ConductId")]
    [StringLength(6)]
    public string? ConductId { get; set; }

    [Column("AcademicPerformanceId")]
    [StringLength(6)]
    public string? AcademicPerformanceId { get; set; }

    [Column("AverageSemester", TypeName = "numeric(10,2)")]
    public decimal AverageSemester { get; set; }

    [ForeignKey("StudentId")]
    public Student Student { get; set; } = null!;

    [ForeignKey("ClassId")]
    public Class Class { get; set; } = null!;

    [ForeignKey("SchoolYearId")]
    public SchoolYear SchoolYear { get; set; } = null!;

    [ForeignKey("SemesterId")]
    public Semester Semester { get; set; } = null!;

    [ForeignKey("ConductId")]
    public Conduct? Conduct { get; set; }

    [ForeignKey("AcademicPerformanceId")]
    public AcademicPerformance? AcademicPerformance { get; set; }
}
