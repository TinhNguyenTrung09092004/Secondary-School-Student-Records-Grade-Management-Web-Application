using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

[Table("RELIGION")]
public class Religion
{
    [Key]
    [Column("ReligionId")]
    [StringLength(6)]
    public string ReligionId { get; set; } = null!;

    [Column("ReligionName")]
    [StringLength(30)]
    public string ReligionName { get; set; } = null!;

    public ICollection<Student> Students { get; set; } = new List<Student>();
}
