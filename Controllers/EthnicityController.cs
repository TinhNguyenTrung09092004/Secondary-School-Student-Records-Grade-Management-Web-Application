using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Services;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "AcademicAffairs")]
public class EthnicityController : ControllerBase
{
    private readonly IEthnicityService _ethnicityService;

    public EthnicityController(IEthnicityService ethnicityService)
    {
        _ethnicityService = ethnicityService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var ethnicityList = await _ethnicityService.GetAllEthnicityAsync();
        return Ok(ethnicityList);
    }

    [HttpGet("{ethnicityId}")]
    public async Task<IActionResult> GetById(string ethnicityId)
    {
        var ethnicity = await _ethnicityService.GetEthnicityByIdAsync(ethnicityId);
        if (ethnicity == null)
        {
            return NotFound(new { message = "Dân tộc không tồn tại" });
        }
        return Ok(ethnicity);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEthnicityDto createDto)
    {
        var ethnicity = await _ethnicityService.CreateEthnicityAsync(createDto);
        if (ethnicity == null)
        {
            return BadRequest(new { message = "Tên dân tộc đã tồn tại" });
        }
        return CreatedAtAction(nameof(GetById), new { ethnicityId = ethnicity.EthnicityId }, ethnicity);
    }

    [HttpPut("{ethnicityId}")]
    public async Task<IActionResult> Update(string ethnicityId, [FromBody] UpdateEthnicityDto updateDto)
    {
        var ethnicity = await _ethnicityService.UpdateEthnicityAsync(ethnicityId, updateDto);
        if (ethnicity == null)
        {
            return NotFound(new { message = "Dân tộc không tồn tại" });
        }
        return Ok(ethnicity);
    }

    [HttpDelete("{ethnicityId}")]
    public async Task<IActionResult> Delete(string ethnicityId)
    {
        var result = await _ethnicityService.DeleteEthnicityAsync(ethnicityId);
        if (!result)
        {
            return BadRequest(new { message = "Không thể xóa dân tộc. Có thể dân tộc này đang được sử dụng bởi học sinh." });
        }
        return Ok(new { message = "Dân tộc đã được xóa thành công" });
    }
}
