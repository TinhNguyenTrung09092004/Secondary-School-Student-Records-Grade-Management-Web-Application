using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

[Table("STUDENT_YEAR_RESULT")]
public class StudentYearResult
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

    [Column("AcademicPerformanceId")]
    [StringLength(6)]
    public string AcademicPerformanceId { get; set; } = null!;

    [Column("ConductId")]
    [StringLength(6)]
    public string ConductId { get; set; } = null!;

    [Column("ResultId")]
    [StringLength(6)]
    public string ResultId { get; set; } = null!;

    [Column("AverageSemester1", TypeName = "numeric(10,2)")]
    public decimal AverageSemester1 { get; set; }

    [Column("AverageSemester2", TypeName = "numeric(10,2)")]
    public decimal AverageSemester2 { get; set; }

    [Column("AverageYear", TypeName = "numeric(10,2)")]
    public decimal AverageYear { get; set; }

    [ForeignKey("StudentId")]
    public Student Student { get; set; } = null!;

    [ForeignKey("ClassId")]
    public Class Class { get; set; } = null!;

    [ForeignKey("SchoolYearId")]
    public SchoolYear SchoolYear { get; set; } = null!;

    [ForeignKey("AcademicPerformanceId")]
    public AcademicPerformance AcademicPerformance { get; set; } = null!;

    [ForeignKey("ConductId")]
    public Conduct Conduct { get; set; } = null!;

    [ForeignKey("ResultId")]
    public Result Result { get; set; } = null!;
}
