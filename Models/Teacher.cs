using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace API.Models;

[Table("TEACHER")]
public class Teacher
{
    [Key]
    [Column("TeacherId")]
    [StringLength(6)]
    public string TeacherId { get; set; } = null!;

    [Column("UserId")]
    public string? UserId { get; set; }

    [ForeignKey("UserId")]
    public ApplicationUser? User { get; set; }

    [Column("TeacherName")]
    [StringLength(30)]
    public string TeacherName { get; set; } = null!;

    [Column("Address")]
    [StringLength(50)]
    public string Address { get; set; } = null!;

    [Column("PhoneNumber")]
    [StringLength(15)]
    public string PhoneNumber { get; set; } = null!;

    [Column("SubjectId")]
    [StringLength(6)]
    public string SubjectId { get; set; } = null!;

    [ForeignKey("SubjectId")]
    public Subject Subject { get; set; } = null!;

    [Column("DepartmentId")]
    [StringLength(6)]
    public string? DepartmentId { get; set; }

    [ForeignKey("DepartmentId")]
    public Department? Department { get; set; }

    public ICollection<Class> Classes { get; set; } = new List<Class>();
    public ICollection<TeachingAssignment> TeachingAssignments { get; set; } = new List<TeachingAssignment>();
}
