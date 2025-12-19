using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

[Table("CLASS_ASSIGNMENT")]
public class ClassAssignment
{
    [Column("SchoolYearId")]
    [StringLength(6)]
    public string SchoolYearId { get; set; } = null!;

    [Column("GradeLevelId")]
    [StringLength(6)]
    public string GradeLevelId { get; set; } = null!;

    [Column("ClassId")]
    [StringLength(10)]
    public string ClassId { get; set; } = null!;

    [Column("StudentId")]
    [StringLength(6)]
    public string StudentId { get; set; } = null!;

    [ForeignKey("SchoolYearId")]
    public SchoolYear SchoolYear { get; set; } = null!;

    [ForeignKey("GradeLevelId")]
    public GradeLevel GradeLevel { get; set; } = null!;

    [ForeignKey("ClassId")]
    public Class Class { get; set; } = null!;

    [ForeignKey("StudentId")]
    public Student Student { get; set; } = null!;
}
