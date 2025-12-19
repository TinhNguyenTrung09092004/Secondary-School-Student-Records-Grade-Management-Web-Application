using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

[Table("DEPARTMENT")]
public class Department
{
    [Key]
    [Column("DepartmentId")]
    [StringLength(6)]
    public string DepartmentId { get; set; } = null!;

    [Column("DepartmentName")]
    [StringLength(50)]
    public string DepartmentName { get; set; } = null!;

    [Column("HeadTeacherId")]
    [StringLength(6)]
    public string? HeadTeacherId { get; set; }

    [ForeignKey("HeadTeacherId")]
    public Teacher? HeadTeacher { get; set; }

    public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
}
