using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

[Table("CLASS_SEMESTER_RESULT")]
public class ClassSemesterResult
{
    [Column("ClassId")]
    [StringLength(10)]
    public string ClassId { get; set; } = null!;

    [Column("SchoolYearId")]
    [StringLength(6)]
    public string SchoolYearId { get; set; } = null!;

    [Column("SemesterId")]
    [StringLength(3)]
    public string SemesterId { get; set; } = null!;

    [Column("PassCount")]
    public int PassCount { get; set; }

    [Column("PassRate", TypeName = "numeric(10,2)")]
    public decimal PassRate { get; set; }

    [ForeignKey("ClassId")]
    public Class Class { get; set; } = null!;

    [ForeignKey("SchoolYearId")]
    public SchoolYear SchoolYear { get; set; } = null!;

    [ForeignKey("SemesterId")]
    public Semester Semester { get; set; } = null!;
}
