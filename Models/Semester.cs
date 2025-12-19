using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

[Table("SEMESTER")]
public class Semester
{
    [Key]
    [Column("SemesterId")]
    [StringLength(3)]
    public string SemesterId { get; set; } = null!;

    [Column("SemesterName")]
    [StringLength(30)]
    public string SemesterName { get; set; } = null!;

    [Column("Coefficient")]
    public int Coefficient { get; set; }

    public ICollection<Grade> Grades { get; set; } = new List<Grade>();
    public ICollection<StudentSubjectResult> StudentSubjectResults { get; set; } = new List<StudentSubjectResult>();
    public ICollection<ClassSubjectResult> ClassSubjectResults { get; set; } = new List<ClassSubjectResult>();
    public ICollection<ClassSemesterResult> ClassSemesterResults { get; set; } = new List<ClassSemesterResult>();
}
