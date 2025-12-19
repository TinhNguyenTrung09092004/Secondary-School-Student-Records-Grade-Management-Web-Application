using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

[Table("GRADE_LEVEL")]
public class GradeLevel
{
    [Key]
    [Column("GradeLevelId")]
    [StringLength(6)]
    public string GradeLevelId { get; set; } = null!;

    [Column("GradeLevelName")]
    [StringLength(30)]
    public string GradeLevelName { get; set; } = null!;

    public ICollection<Class> Classes { get; set; } = new List<Class>();
    public ICollection<ClassAssignment> ClassAssignments { get; set; } = new List<ClassAssignment>();
}
