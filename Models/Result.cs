using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

[Table("RESULT")]
public class Result
{
    [Key]
    [Column("ResultId")]
    [StringLength(6)]
    public string ResultId { get; set; } = null!;

    [Column("ResultName")]
    [StringLength(30)]
    public string ResultName { get; set; } = null!;

    public ICollection<StudentYearResult> StudentYearResults { get; set; } = new List<StudentYearResult>();
}
