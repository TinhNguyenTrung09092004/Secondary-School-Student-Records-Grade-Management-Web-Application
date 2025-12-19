using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Services;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConductController : ControllerBase
{
    private readonly IConductService _conductService;

    public ConductController(IConductService conductService)
    {
        _conductService = conductService;
    }

    [HttpGet]
    [Authorize(Roles = "Principal,SubjectTeacher")]
    public async Task<IActionResult> GetAll()
    {
        var conductList = await _conductService.GetAllConductAsync();
        return Ok(conductList);
    }

    [HttpGet("{conductId}")]
    [Authorize(Roles = "Principal")]
    public async Task<IActionResult> GetById(string conductId)
    {
        var conduct = await _conductService.GetConductByIdAsync(conductId);
        if (conduct == null)
        {
            return NotFound(new { message = "Hạnh kiểm không tồn tại" });
        }
        return Ok(conduct);
    }

    [HttpPost]
    [Authorize(Roles = "Principal")]
    public async Task<IActionResult> Create([FromBody] CreateConductDto createDto)
    {
        var conduct = await _conductService.CreateConductAsync(createDto);
        if (conduct == null)
        {
            return BadRequest(new { message = "Tên hạnh kiểm đã tồn tại" });
        }
        return CreatedAtAction(nameof(GetById), new { conductId = conduct.ConductId }, conduct);
    }

    [HttpPut("{conductId}")]
    [Authorize(Roles = "Principal")]
    public async Task<IActionResult> Update(string conductId, [FromBody] UpdateConductDto updateDto)
    {
        var conduct = await _conductService.UpdateConductAsync(conductId, updateDto);
        if (conduct == null)
        {
            return NotFound(new { message = "Hạnh kiểm không tồn tại" });
        }
        return Ok(conduct);
    }

    [HttpDelete("{conductId}")]
    [Authorize(Roles = "Principal")]
    public async Task<IActionResult> Delete(string conductId)
    {
        var result = await _conductService.DeleteConductAsync(conductId);
        if (!result)
        {
            return BadRequest(new { message = "Không thể xóa hạnh kiểm. Có thể hạnh kiểm này đang được sử dụng bởi học sinh." });
        }
        return Ok(new { message = "Hạnh kiểm đã được xóa thành công" });
    }
}