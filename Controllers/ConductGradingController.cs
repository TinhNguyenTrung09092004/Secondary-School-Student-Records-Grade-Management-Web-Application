using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Services;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SubjectTeacher")]
public class ConductGradingController : ControllerBase
{
    private readonly IConductGradingService _conductGradingService;

    public ConductGradingController(IConductGradingService conductGradingService)
    {
        _conductGradingService = conductGradingService;
    }

    [HttpGet("my-classes")]
    public async Task<IActionResult> GetMyClasses()
    {
        var teacherId = User.FindFirst("TeacherId")?.Value;
        if (string.IsNullOrEmpty(teacherId))
        {
            return BadRequest(new { message = "Không tìm thấy mã giáo viên" });
        }

        var classes = await _conductGradingService.GetTeacherClassesAsync(teacherId);
        return Ok(classes);
    }

    [HttpGet("students")]
    public async Task<IActionResult> GetStudentsForGrading([FromQuery] string classId, [FromQuery] string schoolYearId)
    {
        var teacherId = User.FindFirst("TeacherId")?.Value;
        if (string.IsNullOrEmpty(teacherId))
        {
            return BadRequest(new { message = "Không tìm thấy mã giáo viên" });
        }

        if (string.IsNullOrEmpty(classId) || string.IsNullOrEmpty(schoolYearId))
        {
            return BadRequest(new { message = "Thiếu thông tin lớp học hoặc năm học" });
        }

        var students = await _conductGradingService.GetStudentsForConductGradingAsync(classId, schoolYearId, teacherId);
        if (students.Count == 0)
        {
            return NotFound(new { message = "Không tìm thấy học sinh hoặc bạn không phải giáo viên chủ nhiệm của lớp này" });
        }

        return Ok(students);
    }

    [HttpPut("update-conduct")]
    public async Task<IActionResult> UpdateStudentConduct([FromBody] UpdateStudentConductDto updateDto)
    {
        var teacherId = User.FindFirst("TeacherId")?.Value;
        if (string.IsNullOrEmpty(teacherId))
        {
            return BadRequest(new { message = "Không tìm thấy mã giáo viên" });
        }

        try
        {
            var result = await _conductGradingService.UpdateStudentConductAsync(updateDto, teacherId);
            return Ok(new { message = "Cập nhật hạnh kiểm thành công" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}