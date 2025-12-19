using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

[Table("CLASS")]
public class Class
{
    [Key]
    [Column("ClassId")]
    [StringLength(10)]
    public string ClassId { get; set; } = null!;

    [Column("ClassName")]
    [StringLength(30)]
    public string ClassName { get; set; } = null!;

    [Column("GradeLevelId")]
    [StringLength(6)]
    public string GradeLevelId { get; set; } = null!;

    [Column("SchoolYearId")]
    [StringLength(6)]
    public string SchoolYearId { get; set; } = null!;

    [Column("ClassSize")]
    public int ClassSize { get; set; }

    [Column("TeacherId")]
    [StringLength(6)]
    public string? TeacherId { get; set; }

    [ForeignKey("GradeLevelId")]
    public GradeLevel GradeLevel { get; set; } = null!;

    [ForeignKey("SchoolYearId")]
    public SchoolYear SchoolYear { get; set; } = null!;

    [ForeignKey("TeacherId")]
    public Teacher? Teacher { get; set; }

    public ICollection<ClassAssignment> ClassAssignments { get; set; } = new List<ClassAssignment>();
    public ICollection<TeachingAssignment> TeachingAssignments { get; set; } = new List<TeachingAssignment>();
    public ICollection<Grade> Grades { get; set; } = new List<Grade>();
    public ICollection<StudentSubjectResult> StudentSubjectResults { get; set; } = new List<StudentSubjectResult>();
    public ICollection<StudentYearResult> StudentYearResults { get; set; } = new List<StudentYearResult>();
    public ICollection<ClassSubjectResult> ClassSubjectResults { get; set; } = new List<ClassSubjectResult>();
    public ICollection<ClassSemesterResult> ClassSemesterResults { get; set; } = new List<ClassSemesterResult>();
}
