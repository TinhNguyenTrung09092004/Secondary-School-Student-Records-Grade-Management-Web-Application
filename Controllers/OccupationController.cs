using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Services;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "AcademicAffairs")]
public class OccupationController : ControllerBase
{
    private readonly IOccupationService _occupationService;

    public OccupationController(IOccupationService occupationService)
    {
        _occupationService = occupationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var occupationList = await _occupationService.GetAllOccupationAsync();
        return Ok(occupationList);
    }

    [HttpGet("{occupationId}")]
    public async Task<IActionResult> GetById(string occupationId)
    {
        var occupation = await _occupationService.GetOccupationByIdAsync(occupationId);
        if (occupation == null)
        {
            return NotFound(new { message = "Nghề nghiệp không tồn tại" });
        }
        return Ok(occupation);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOccupationDto createDto)
    {
        var occupation = await _occupationService.CreateOccupationAsync(createDto);
        if (occupation == null)
        {
            return BadRequest(new { message = "Tên nghề nghiệp đã tồn tại" });
        }
        return CreatedAtAction(nameof(GetById), new { occupationId = occupation.OccupationId }, occupation);
    }

    [HttpPut("{occupationId}")]
    public async Task<IActionResult> Update(string occupationId, [FromBody] UpdateOccupationDto updateDto)
    {
        var occupation = await _occupationService.UpdateOccupationAsync(occupationId, updateDto);
        if (occupation == null)
        {
            return NotFound(new { message = "Nghề nghiệp không tồn tại" });
        }
        return Ok(occupation);
    }

    [HttpDelete("{occupationId}")]
    public async Task<IActionResult> Delete(string occupationId)
    {
        var result = await _occupationService.DeleteOccupationAsync(occupationId);
        if (!result)
        {
            return BadRequest(new { message = "Không thể xóa nghề nghiệp. Có thể nghề nghiệp này đang được sử dụng bởi học sinh." });
        }
        return Ok(new { message = "Nghề nghiệp đã được xóa thành công" });
    }
}
