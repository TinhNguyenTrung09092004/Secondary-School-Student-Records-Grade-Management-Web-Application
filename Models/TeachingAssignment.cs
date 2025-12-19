using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

[Table("TEACHING_ASSIGNMENT")]
public class TeachingAssignment
{
    [Key]
    [Column("RowNumber")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RowNumber { get; set; }

    [Column("SchoolYearId")]
    [StringLength(6)]
    public string SchoolYearId { get; set; } = null!;

    [Column("ClassId")]
    [StringLength(10)]
    public string ClassId { get; set; } = null!;

    [Column("SubjectId")]
    [StringLength(6)]
    public string SubjectId { get; set; } = null!;

    [Column("TeacherId")]
    [StringLength(6)]
    public string TeacherId { get; set; } = null!;

    [ForeignKey("SchoolYearId")]
    public SchoolYear SchoolYear { get; set; } = null!;

    [ForeignKey("ClassId")]
    public Class Class { get; set; } = null!;

    [ForeignKey("SubjectId")]
    public Subject Subject { get; set; } = null!;

    [ForeignKey("TeacherId")]
    public Teacher Teacher { get; set; } = null!;
}
