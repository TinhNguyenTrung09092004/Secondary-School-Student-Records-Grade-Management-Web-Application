using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class GradeTypeDto
{
    public string GradeTypeId { get; set; } = null!;
    public string GradeTypeName { get; set; } = null!;
    public int Coefficient { get; set; }
}

public class CreateGradeTypeDto
{
    [Required(ErrorMessage = "Mã loại điểm là bắt buộc")]
    [StringLength(6, ErrorMessage = "Mã loại điểm không được vượt quá 6 ký tự")]
    public string GradeTypeId { get; set; } = null!;

    [Required(ErrorMessage = "Tên loại điểm là bắt buộc")]
    [StringLength(30, ErrorMessage = "Tên loại điểm không được vượt quá 30 ký tự")]
    public string GradeTypeName { get; set; } = null!;

    [Required(ErrorMessage = "Hệ số là bắt buộc")]
    [Range(1, int.MaxValue, ErrorMessage = "Hệ số phải lớn hơn 0")]
    public int Coefficient { get; set; }
}

public class UpdateGradeTypeDto
{
    [Required(ErrorMessage = "Mã loại điểm là bắt buộc")]
    [StringLength(6, ErrorMessage = "Mã loại điểm không được vượt quá 6 ký tự")]
    public string GradeTypeId { get; set; } = null!;

    [Required(ErrorMessage = "Tên loại điểm là bắt buộc")]
    [StringLength(30, ErrorMessage = "Tên loại điểm không được vượt quá 30 ký tự")]
    public string GradeTypeName { get; set; } = null!;

    [Required(ErrorMessage = "Hệ số là bắt buộc")]
    [Range(1, int.MaxValue, ErrorMessage = "Hệ số phải lớn hơn 0")]
    public int Coefficient { get; set; }
}
