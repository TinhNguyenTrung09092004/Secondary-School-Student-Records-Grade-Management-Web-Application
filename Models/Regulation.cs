using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

[Table("REGULATION")]
public class Regulation
{
    [Column("MinClassSize")]
    public int MinClassSize { get; set; }

    [Column("MaxClassSize")]
    public int MaxClassSize { get; set; }

    [Column("PassingScore")]
    public int PassingScore { get; set; }
}
