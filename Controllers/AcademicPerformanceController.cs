using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Services;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "AcademicAffairs,Principal")]
public class AcademicPerformanceController : ControllerBase
{
    private readonly IAcademicPerformanceService _service;

    public AcademicPerformanceController(IAcademicPerformanceService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var academicPerformances = await _service.GetAllAcademicPerformancesAsync();
        return Ok(academicPerformances);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var academicPerformance = await _service.GetAcademicPerformanceByIdAsync(id);
        if (academicPerformance == null)
        {
            return NotFound(new { message = "Không tìm thấy học lực" });
        }
        return Ok(academicPerformance);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAcademicPerformanceDto createDto)
    {
        try
        {
            var academicPerformance = await _service.CreateAcademicPerformanceAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = academicPerformance.AcademicPerformanceId }, academicPerformance);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateAcademicPerformanceDto updateDto)
    {
        try
        {
            var academicPerformance = await _service.UpdateAcademicPerformanceAsync(id, updateDto);
            return Ok(academicPerformance);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await _service.DeleteAcademicPerformanceAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
