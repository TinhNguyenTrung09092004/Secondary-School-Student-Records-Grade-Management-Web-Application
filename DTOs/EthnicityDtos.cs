using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class EthnicityDto
{
    public string EthnicityId { get; set; } = null!;
    public string EthnicityName { get; set; } = null!;
}

public class CreateEthnicityDto
{
    [Required(ErrorMessage = "Tên dân tộc là bắt buộc")]
    [StringLength(30, ErrorMessage = "Tên dân tộc không được vượt quá 30 ký tự")]
    public string EthnicityName { get; set; } = null!;
}

public class UpdateEthnicityDto
{
    [Required(ErrorMessage = "Tên dân tộc là bắt buộc")]
    [StringLength(30, ErrorMessage = "Tên dân tộc không được vượt quá 30 ký tự")]
    public string EthnicityName { get; set; } = null!;
}
