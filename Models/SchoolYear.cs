using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

[Table("SCHOOL_YEAR")]
public class SchoolYear
{
    [Key]
    [Column("SchoolYearId")]
    [StringLength(6)]
    public string SchoolYearId { get; set; } = null!;

    [Column("SchoolYearName")]
    [StringLength(30)]
    public string SchoolYearName { get; set; } = null!;

    public ICollection<Class> Classes { get; set; } = new List<Class>();
    public ICollection<ClassAssignment> ClassAssignments { get; set; } = new List<ClassAssignment>();
    public ICollection<TeachingAssignment> TeachingAssignments { get; set; } = new List<TeachingAssignment>();
    public ICollection<Grade> Grades { get; set; } = new List<Grade>();
    public ICollection<StudentSubjectResult> StudentSubjectResults { get; set; } = new List<StudentSubjectResult>();
    public ICollection<StudentYearResult> StudentYearResults { get; set; } = new List<StudentYearResult>();
    public ICollection<ClassSubjectResult> ClassSubjectResults { get; set; } = new List<ClassSubjectResult>();
    public ICollection<ClassSemesterResult> ClassSemesterResults { get; set; } = new List<ClassSemesterResult>();
}
