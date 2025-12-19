using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

[Table("CONDUCT")]
public class Conduct
{
    [Key]
    [Column("ConductId")]
    [StringLength(6)]
    public string ConductId { get; set; } = null!;

    [Column("ConductName")]
    [StringLength(30)]
    public string ConductName { get; set; } = null!;

    public ICollection<StudentYearResult> StudentYearResults { get; set; } = new List<StudentYearResult>();
}
