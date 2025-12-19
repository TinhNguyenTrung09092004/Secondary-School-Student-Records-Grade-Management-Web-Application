using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Services;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GradeLevelController : ControllerBase
{
    private readonly IGradeLevelService _gradeLevelService;

    public GradeLevelController(IGradeLevelService gradeLevelService)
    {
        _gradeLevelService = gradeLevelService;
    }

    [HttpGet]
    [Authorize(Roles = "AcademicAffairs,Principal,SubjectTeacher")]
    public async Task<IActionResult> GetAll()
    {
        var gradeLevels = await _gradeLevelService.GetAllGradeLevelsAsync();
        return Ok(gradeLevels);
    }

    [HttpGet("{gradeLevelId}")]
    [Authorize(Roles = "AcademicAffairs,Principal,SubjectTeacher")]
    public async Task<IActionResult> GetById(string gradeLevelId)
    {
        var gradeLevel = await _gradeLevelService.GetGradeLevelByIdAsync(gradeLevelId);
        if (gradeLevel == null)
        {
            return NotFound(new { message = "Khối không tồn tại" });
        }
        return Ok(gradeLevel);
    }

    [HttpPost]
    [Authorize(Roles = "AcademicAffairs")]
    public async Task<IActionResult> Create([FromBody] CreateGradeLevelDto createDto)
    {
        var gradeLevel = await _gradeLevelService.CreateGradeLevelAsync(createDto);
        if (gradeLevel == null)
        {
            return BadRequest(new { message = "Mã khối đã tồn tại" });
        }
        return CreatedAtAction(nameof(GetById), new { gradeLevelId = gradeLevel.GradeLevelId }, gradeLevel);
    }

    [HttpPut("{gradeLevelId}")]
    [Authorize(Roles = "AcademicAffairs")]
    public async Task<IActionResult> Update(string gradeLevelId, [FromBody] UpdateGradeLevelDto updateDto)
    {
        var gradeLevel = await _gradeLevelService.UpdateGradeLevelAsync(gradeLevelId, updateDto);
        if (gradeLevel == null)
        {
            return NotFound(new { message = "Khối không tồn tại" });
        }
        return Ok(gradeLevel);
    }

    [HttpDelete("{gradeLevelId}")]
    [Authorize(Roles = "AcademicAffairs")]
    public async Task<IActionResult> Delete(string gradeLevelId)
    {
        var result = await _gradeLevelService.DeleteGradeLevelAsync(gradeLevelId);
        if (!result)
        {
            return BadRequest(new { message = "Không thể xóa khối. Có thể khối này đang có dữ liệu liên quan (lớp, phân công lớp)." });
        }
        return Ok(new { message = "Khối đã được xóa thành công" });
    }
}
