using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

[Table("STUDENT_SUBJECT_RESULT")]
public class StudentSubjectResult
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

    [Column("SubjectId")]
    [StringLength(6)]
    public string SubjectId { get; set; } = null!;

    [Column("SemesterId")]
    [StringLength(3)]
    public string SemesterId { get; set; } = null!;

    [Column("AverageSemester", TypeName = "numeric(10,2)")]
    public decimal? AverageSemester { get; set; }

    [Column("CommentResult")]
    [StringLength(10)]
    public string? CommentResult { get; set; }

    [ForeignKey("StudentId")]
    public Student Student { get; set; } = null!;

    [ForeignKey("ClassId")]
    public Class Class { get; set; } = null!;

    [ForeignKey("SchoolYearId")]
    public SchoolYear SchoolYear { get; set; } = null!;

    [ForeignKey("SubjectId")]
    public Subject Subject { get; set; } = null!;

    [ForeignKey("SemesterId")]
    public Semester Semester { get; set; } = null!;
}
