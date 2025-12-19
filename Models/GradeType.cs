using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

[Table("GRADE_TYPE")]
public class GradeType
{
    [Key]
    [Column("GradeTypeId")]
    [StringLength(6)]
    public string GradeTypeId { get; set; } = null!;

    [Column("GradeTypeName")]
    [StringLength(30)]
    public string GradeTypeName { get; set; } = null!;

    [Column("Coefficient")]
    public int Coefficient { get; set; }

    public ICollection<Grade> Grades { get; set; } = new List<Grade>();
}
