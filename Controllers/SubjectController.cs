using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Services;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubjectController : ControllerBase
{
    private readonly ISubjectService _subjectService;

    public SubjectController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    [HttpGet]
    [Authorize(Roles = "Principal,SubjectTeacher")]
    public async Task<IActionResult> GetAll()
    {
        var subjectList = await _subjectService.GetAllSubjectsAsync();
        return Ok(subjectList);
    }

    [HttpGet("{subjectId}")]
    [Authorize(Roles = "Principal,SubjectTeacher")]
    public async Task<IActionResult> GetById(string subjectId)
    {
        var subject = await _subjectService.GetSubjectByIdAsync(subjectId);
        if (subject == null)
        {
            return NotFound(new { message = "Môn học không tồn tại" });
        }
        return Ok(subject);
    }

    [HttpPost]
    [Authorize(Roles = "Principal")]
    public async Task<IActionResult> Create([FromBody] CreateSubjectDto createDto)
    {
        var subject = await _subjectService.CreateSubjectAsync(createDto);
        if (subject == null)
        {
            return BadRequest(new { message = "Tên môn học đã tồn tại" });
        }
        return CreatedAtAction(nameof(GetById), new { subjectId = subject.SubjectId }, subject);
    }

    [HttpPut("{subjectId}")]
    [Authorize(Roles = "Principal")]
    public async Task<IActionResult> Update(string subjectId, [FromBody] UpdateSubjectDto updateDto)
    {
        var subject = await _subjectService.UpdateSubjectAsync(subjectId, updateDto);
        if (subject == null)
        {
            return NotFound(new { message = "Môn học không tồn tại" });
        }
        return Ok(subject);
    }

    [HttpDelete("{subjectId}")]
    [Authorize(Roles = "Principal")]
    public async Task<IActionResult> Delete(string subjectId)
    {
        var result = await _subjectService.DeleteSubjectAsync(subjectId);
        if (!result)
        {
            return BadRequest(new { message = "Không thể xóa môn học. Có thể môn học này đang được sử dụng." });
        }
        return Ok(new { message = "Môn học đã được xóa thành công" });
    }
}
