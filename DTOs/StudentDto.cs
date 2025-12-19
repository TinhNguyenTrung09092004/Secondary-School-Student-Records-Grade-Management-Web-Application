using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class StudentDto
{
    public string StudentId { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public bool Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Address { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string EthnicityId { get; set; } = null!;
    public string ReligionId { get; set; } = null!;
    public string FatherName { get; set; } = null!;
    public string FatherOccupationId { get; set; } = null!;
    public string MotherName { get; set; } = null!;
    public string MotherOccupationId { get; set; } = null!;
}

public class CreateStudentDto
{
    [Required(ErrorMessage = "Mã học sinh là bắt buộc")]
    [StringLength(6, ErrorMessage = "Mã học sinh không được vượt quá 6 ký tự")]
    public string StudentId { get; set; } = null!;

    [Required(ErrorMessage = "Họ tên là bắt buộc")]
    [StringLength(30, ErrorMessage = "Họ tên không được vượt quá 30 ký tự")]
    public string FullName { get; set; } = null!;

    [Required]
    public bool Gender { get; set; }

    [Required(ErrorMessage = "Ngày sinh là bắt buộc")]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
    [StringLength(50, ErrorMessage = "Địa chỉ không được vượt quá 50 ký tự")]
    public string Address { get; set; } = null!;

    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [StringLength(50, ErrorMessage = "Email không được vượt quá 50 ký tự")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Dân tộc là bắt buộc")]
    [StringLength(6)]
    public string EthnicityId { get; set; } = null!;

    [Required(ErrorMessage = "Tôn giáo là bắt buộc")]
    [StringLength(6)]
    public string ReligionId { get; set; } = null!;

    [Required(ErrorMessage = "Tên cha là bắt buộc")]
    [StringLength(30, ErrorMessage = "Tên cha không được vượt quá 30 ký tự")]
    public string FatherName { get; set; } = null!;

    [Required(ErrorMessage = "Nghề nghiệp cha là bắt buộc")]
    [StringLength(6)]
    public string FatherOccupationId { get; set; } = null!;

    [Required(ErrorMessage = "Tên mẹ là bắt buộc")]
    [StringLength(30, ErrorMessage = "Tên mẹ không được vượt quá 30 ký tự")]
    public string MotherName { get; set; } = null!;

    [Required(ErrorMessage = "Nghề nghiệp mẹ là bắt buộc")]
    [StringLength(6)]
    public string MotherOccupationId { get; set; } = null!;
}

public class UpdateStudentDto
{
    [Required(ErrorMessage = "Họ tên là bắt buộc")]
    [StringLength(30, ErrorMessage = "Họ tên không được vượt quá 30 ký tự")]
    public string FullName { get; set; } = null!;

    [Required]
    public bool Gender { get; set; }

    [Required(ErrorMessage = "Ngày sinh là bắt buộc")]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
    [StringLength(50, ErrorMessage = "Địa chỉ không được vượt quá 50 ký tự")]
    public string Address { get; set; } = null!;

    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [StringLength(50, ErrorMessage = "Email không được vượt quá 50 ký tự")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Dân tộc là bắt buộc")]
    [StringLength(6)]
    public string EthnicityId { get; set; } = null!;

    [Required(ErrorMessage = "Tôn giáo là bắt buộc")]
    [StringLength(6)]
    public string ReligionId { get; set; } = null!;

    [Required(ErrorMessage = "Tên cha là bắt buộc")]
    [StringLength(30, ErrorMessage = "Tên cha không được vượt quá 30 ký tự")]
    public string FatherName { get; set; } = null!;

    [Required(ErrorMessage = "Nghề nghiệp cha là bắt buộc")]
    [StringLength(6)]
    public string FatherOccupationId { get; set; } = null!;

    [Required(ErrorMessage = "Tên mẹ là bắt buộc")]
    [StringLength(30, ErrorMessage = "Tên mẹ không được vượt quá 30 ký tự")]
    public string MotherName { get; set; } = null!;

    [Required(ErrorMessage = "Nghề nghiệp mẹ là bắt buộc")]
    [StringLength(6)]
    public string MotherOccupationId { get; set; } = null!;
}
