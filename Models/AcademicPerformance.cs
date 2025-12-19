using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

[Table("ACADEMIC_PERFORMANCE")]
public class AcademicPerformance
{
    [Key]
    [Column("AcademicPerformanceId")]
    [StringLength(6)]
    public string AcademicPerformanceId { get; set; } = null!;

    [Column("AcademicPerformanceName")]
    [StringLength(30)]
    public string AcademicPerformanceName { get; set; } = null!;

    public ICollection<StudentYearResult> StudentYearResults { get; set; } = new List<StudentYearResult>();
}
