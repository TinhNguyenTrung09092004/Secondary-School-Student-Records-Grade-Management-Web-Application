using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace API.Models;

[Table("STUDENT")]
public class Student
{
    [Key]
    [Column("StudentId")]
    [StringLength(6)]
    public string StudentId { get; set; } = null!;

    [Column("UserId")]
    public string? UserId { get; set; }

    [ForeignKey("UserId")]
    public ApplicationUser? User { get; set; }

    [Column("FullName")]
    [StringLength(30)]
    public string FullName { get; set; } = null!;

    [Column("Gender")]
    public bool Gender { get; set; }

    [Column("DateOfBirth")]
    public DateTime DateOfBirth { get; set; }

    [Column("Address")]
    [StringLength(50)]
    public string Address { get; set; } = null!;

    [Column("EthnicityId")]
    [StringLength(6)]
    public string EthnicityId { get; set; } = null!;

    [Column("ReligionId")]
    [StringLength(6)]
    public string ReligionId { get; set; } = null!;

    [Column("FatherName")]
    [StringLength(30)]
    public string FatherName { get; set; } = null!;

    [Column("FatherOccupationId")]
    [StringLength(6)]
    public string FatherOccupationId { get; set; } = null!;

    [Column("MotherName")]
    [StringLength(30)]
    public string MotherName { get; set; } = null!;

    [Column("MotherOccupationId")]
    [StringLength(6)]
    public string MotherOccupationId { get; set; } = null!;

    [Column("Email")]
    [StringLength(50)]
    public string Email { get; set; } = null!;

    [ForeignKey("EthnicityId")]
    public Ethnicity Ethnicity { get; set; } = null!;

    [ForeignKey("ReligionId")]
    public Religion Religion { get; set; } = null!;

    [ForeignKey("FatherOccupationId")]
    public Occupation FatherOccupation { get; set; } = null!;

    [ForeignKey("MotherOccupationId")]
    public Occupation MotherOccupation { get; set; } = null!;

    public ICollection<ClassAssignment> ClassAssignments { get; set; } = new List<ClassAssignment>();
    public ICollection<Grade> Grades { get; set; } = new List<Grade>();
    public ICollection<StudentSubjectResult> StudentSubjectResults { get; set; } = new List<StudentSubjectResult>();
    public ICollection<StudentYearResult> StudentYearResults { get; set; } = new List<StudentYearResult>();
}
