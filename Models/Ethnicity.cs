using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

[Table("ETHNICITY")]
public class Ethnicity
{
    [Key]
    [Column("EthnicityId")]
    [StringLength(6)]
    public string EthnicityId { get; set; } = null!;

    [Column("EthnicityName")]
    [StringLength(30)]
    public string EthnicityName { get; set; } = null!;

    public ICollection<Student> Students { get; set; } = new List<Student>();
}