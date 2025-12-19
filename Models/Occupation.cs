using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

[Table("OCCUPATION")]
public class Occupation
{
    [Key]
    [Column("OccupationId")]
    [StringLength(6)]
    public string OccupationId { get; set; } = null!;

    [Column("OccupationName")]
    [StringLength(30)]
    public string OccupationName { get; set; } = null!;

    [InverseProperty("FatherOccupation")]
    public ICollection<Student> StudentFathers { get; set; } = new List<Student>();

    [InverseProperty("MotherOccupation")]
    public ICollection<Student> StudentMothers { get; set; } = new List<Student>();
}
