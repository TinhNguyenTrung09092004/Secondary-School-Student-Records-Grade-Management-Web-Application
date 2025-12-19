using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Services;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeacherController : ControllerBase
{
    private readonly ITeacherService _teacherService;

    public TeacherController(ITeacherService teacherService)
    {
        _teacherService = teacherService;
    }

    [HttpGet]
    [Authorize(Roles = "AcademicAffairs,Principal,SubjectTeacher")]
    public async Task<IActionResult> GetAllTeachers()
    {
        var teachers = await _teacherService.GetAllTeachersAsync();
        return Ok(teachers);
    }

    [HttpGet("profile")]
    [Authorize(Roles = "SubjectTeacher,Principal")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        var teacher = await _teacherService.GetTeacherProfileByUserIdAsync(userId);
        if (teacher == null)
        {
            return NotFound(new { message = "Hồ sơ giáo viên không tồn tại" });
        }

        return Ok(teacher);
    }

    [HttpPost("profile")]
    [Authorize(Roles = "SubjectTeacher")]
    public async Task<IActionResult> CreateProfile([FromBody] CreateTeacherProfileDto createDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        var teacher = await _teacherService.CreateTeacherProfileAsync(userId, createDto);
        if (teacher == null)
        {
            return BadRequest(new { message = "Không thể tạo hồ sơ. Mã giáo viên đã tồn tại, bạn đã có hồ sơ, hoặc môn học/phòng ban không hợp lệ" });
        }

        return CreatedAtAction(nameof(GetProfile), teacher);
    }

    [HttpPut("profile")]
    [Authorize(Roles = "SubjectTeacher")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateTeacherProfileDto updateDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        var teacher = await _teacherService.UpdateTeacherProfileAsync(userId, updateDto);
        if (teacher == null)
        {
            return BadRequest(new { message = "Không thể cập nhật hồ sơ. Hồ sơ không tồn tại hoặc môn học/phòng ban không hợp lệ" });
        }

        return Ok(teacher);
    }

    [HttpDelete("profile")]
    [Authorize(Roles = "SubjectTeacher")]
    public async Task<IActionResult> DeleteProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        var result = await _teacherService.DeleteTeacherProfileAsync(userId);
        if (!result)
        {
            return BadRequest(new { message = "Không thể xóa hồ sơ. Có thể hồ sơ không tồn tại hoặc bạn đang có dữ liệu liên quan (lớp, phân công giảng dạy)." });
        }

        return Ok(new { message = "Hồ sơ giáo viên đã được xóa thành công" });
    }
}
