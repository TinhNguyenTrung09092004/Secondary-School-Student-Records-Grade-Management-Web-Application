using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

[Table("SUBJECT")]
public class Subject
{
    [Key]
    [Column("SubjectId")]
    [StringLength(6)]
    public string SubjectId { get; set; } = null!;

    [Column("SubjectName")]
    [StringLength(30)]
    public string SubjectName { get; set; } = null!;

    [Column("LessonCount")]
    public int LessonCount { get; set; }

    [Column("Coefficient")]
    public int Coefficient { get; set; }

    public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
    public ICollection<TeachingAssignment> TeachingAssignments { get; set; } = new List<TeachingAssignment>();
    public ICollection<Grade> Grades { get; set; } = new List<Grade>();
    public ICollection<StudentSubjectResult> StudentSubjectResults { get; set; } = new List<StudentSubjectResult>();
    public ICollection<ClassSubjectResult> ClassSubjectResults { get; set; } = new List<ClassSubjectResult>();
}
