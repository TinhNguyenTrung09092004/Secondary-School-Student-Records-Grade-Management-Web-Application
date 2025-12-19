using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Services;
using API.DTOs;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClassController : ControllerBase
{
    private readonly IClassService _classService;

    public ClassController(IClassService classService)
    {
        _classService = classService;
    }

    [HttpGet]
    [Authorize(Roles = "AcademicAffairs,Principal,SubjectTeacher")]
    public async Task<IActionResult> GetAllClasses()
    {
        var classes = await _classService.GetAllClassesAsync();
        return Ok(classes);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "AcademicAffairs,Principal,SubjectTeacher")]
    public async Task<IActionResult> GetClassById(string id)
    {
        var classDto = await _classService.GetClassByIdAsync(id);
        if (classDto == null)
            return NotFound(new { message = $"Class with ID {id} not found" });

        return Ok(classDto);
    }

    [HttpGet("by-year-grade")]
    [Authorize(Roles = "AcademicAffairs,Principal")]
    public async Task<IActionResult> GetByYearAndGrade([FromQuery] string schoolYearId, [FromQuery] string gradeLevelId)
    {
        var classes = await _classService.GetClassesBySchoolYearAndGradeLevelAsync(schoolYearId, gradeLevelId);
        return Ok(classes);
    }

    [HttpPost]
    [Authorize(Roles = "AcademicAffairs")]
    public async Task<IActionResult> CreateClass([FromBody] CreateClassDto createClassDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await _classService.ClassExistsAsync(createClassDto.ClassId))
            return Conflict(new { message = $"Class with ID {createClassDto.ClassId} already exists" });

        var classDto = await _classService.CreateClassAsync(createClassDto);
        return CreatedAtAction(nameof(GetClassById), new { id = classDto.ClassId }, classDto);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "AcademicAffairs")]
    public async Task<IActionResult> UpdateClass(string id, [FromBody] UpdateClassDto updateClassDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _classService.ClassExistsAsync(id))
            return NotFound(new { message = $"Class with ID {id} not found" });

        try
        {
            var classDto = await _classService.UpdateClassAsync(id, updateClassDto);
            return Ok(classDto);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{id}/eligible-homeroom-teachers")]
    [Authorize(Roles = "Principal")]
    public async Task<IActionResult> GetEligibleHomeroomTeachers(string id)
    {
        try
        {
            var teachers = await _classService.GetEligibleHomeroomTeachersAsync(id);
            return Ok(teachers);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/assign-teacher")]
    [Authorize(Roles = "Principal")]
    public async Task<IActionResult> AssignTeacher(string id, [FromBody] AssignTeacherDto assignTeacherDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var classDto = await _classService.AssignTeacherToClassAsync(id, assignTeacherDto);
            return Ok(classDto);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "AcademicAffairs")]
    public async Task<IActionResult> DeleteClass(string id)
    {
        var result = await _classService.DeleteClassAsync(id);
        if (!result)
            return NotFound(new { message = $"Class with ID {id} not found" });

        return NoContent();
    }
}
