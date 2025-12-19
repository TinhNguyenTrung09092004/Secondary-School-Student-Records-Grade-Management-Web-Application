using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Services;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "AcademicAffairs")]
public class ReligionController : ControllerBase
{
    private readonly IReligionService _religionService;

    public ReligionController(IReligionService religionService)
    {
        _religionService = religionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var religionList = await _religionService.GetAllReligionAsync();
        return Ok(religionList);
    }

    [HttpGet("{religionId}")]
    public async Task<IActionResult> GetById(string religionId)
    {
        var religion = await _religionService.GetReligionByIdAsync(religionId);
        if (religion == null)
        {
            return NotFound(new { message = "Tôn giáo không tồn tại" });
        }
        return Ok(religion);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReligionDto createDto)
    {
        var religion = await _religionService.CreateReligionAsync(createDto);
        if (religion == null)
        {
            return BadRequest(new { message = "Tên tôn giáo đã tồn tại" });
        }
        return CreatedAtAction(nameof(GetById), new { religionId = religion.ReligionId }, religion);
    }

    [HttpPut("{religionId}")]
    public async Task<IActionResult> Update(string religionId, [FromBody] UpdateReligionDto updateDto)
    {
        var religion = await _religionService.UpdateReligionAsync(religionId, updateDto);
        if (religion == null)
        {
            return NotFound(new { message = "Tôn giáo không tồn tại" });
        }
        return Ok(religion);
    }

    [HttpDelete("{religionId}")]
    public async Task<IActionResult> Delete(string religionId)
    {
        var result = await _religionService.DeleteReligionAsync(religionId);
        if (!result)
        {
            return BadRequest(new { message = "Không thể xóa tôn giáo. Có thể tôn giáo này đang được sử dụng bởi học sinh." });
        }
        return Ok(new { message = "Tôn giáo đã được xóa thành công" });
    }
}
