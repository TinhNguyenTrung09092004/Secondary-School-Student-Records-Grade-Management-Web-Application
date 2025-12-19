using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Services;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SemesterController : ControllerBase
{
    private readonly ISemesterService _semesterService;

    public SemesterController(ISemesterService semesterService)
    {
        _semesterService = semesterService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSemesters()
    {
        var semesters = await _semesterService.GetAllSemestersAsync();
        return Ok(semesters);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Principal")]
    public async Task<IActionResult> GetSemesterById(string id)
    {
        var semester = await _semesterService.GetSemesterByIdAsync(id);
        if (semester == null)
        {
            return NotFound(new { message = "Không tìm thấy học kỳ" });
        }
        return Ok(semester);
    }

    [HttpPost]
    [Authorize(Roles = "Principal")]
    public async Task<IActionResult> CreateSemester([FromBody] CreateSemesterDto createDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var semester = await _semesterService.CreateSemesterAsync(createDto);
        if (semester == null)
        {
            return BadRequest(new { message = "Mã học kỳ hoặc tên học kỳ đã tồn tại" });
        }

        return CreatedAtAction(nameof(GetSemesterById), new { id = semester.SemesterId }, semester);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Principal")]
    public async Task<IActionResult> UpdateSemester(string id, [FromBody] UpdateSemesterDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var semester = await _semesterService.UpdateSemesterAsync(id, updateDto);
            if (semester == null)
            {
                return NotFound(new { message = "Không tìm thấy học kỳ hoặc mã/tên học kỳ đã tồn tại" });
            }

            return Ok(semester);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Principal")]
    public async Task<IActionResult> DeleteSemester(string id)
    {
        var result = await _semesterService.DeleteSemesterAsync(id);
        if (!result)
        {
            return BadRequest(new { message = "Không thể xóa học kỳ. Học kỳ đang được sử dụng hoặc không tồn tại." });
        }

        return NoContent();
    }
}
