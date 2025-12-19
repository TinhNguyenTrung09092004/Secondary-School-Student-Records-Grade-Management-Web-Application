using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class ConductDto
{
    public string ConductId { get; set; } = null!;
    public string ConductName { get; set; } = null!;
}

public class CreateConductDto
{
    [Required(ErrorMessage = "Tên hạnh kiểm là bắt buộc")]
    [StringLength(30, ErrorMessage = "Tên hạnh kiểm không được vượt quá 30 ký tự")]
    public string ConductName { get; set; } = null!;
}

public class UpdateConductDto
{
    [Required(ErrorMessage = "Tên hạnh kiểm là bắt buộc")]
    [StringLength(30, ErrorMessage = "Tên hạnh kiểm không được vượt quá 30 ký tự")]
    public string ConductName { get; set; } = null!;
}