using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Services;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SubjectTeacher")]
public class TeachingAssignmentController : ControllerBase
{
    private readonly ITeachingAssignmentService _teachingAssignmentService;

    public TeachingAssignmentController(ITeachingAssignmentService teachingAssignmentService)
    {
        _teachingAssignmentService = teachingAssignmentService;
    }

    [HttpGet("department-head/check")]
    public async Task<IActionResult> CheckIsDepartmentHead()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        var isDepartmentHead = await _teachingAssignmentService.IsDepartmentHeadAsync(userId);
        return Ok(new { isDepartmentHead });
    }

    [HttpGet("department-head/teachers")]
    public async Task<IActionResult> GetDepartmentTeachers()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        var teachers = await _teachingAssignmentService.GetDepartmentTeachersAsync(userId);
        if (!teachers.Any())
        {
            return Forbidden(new { message = "Bạn không phải trưởng khoa hoặc không có quyền truy cập" });
        }

        return Ok(teachers);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTeachingAssignments()
    {
        var assignments = await _teachingAssignmentService.GetAllTeachingAssignmentsAsync();
        return Ok(assignments);
    }

    [HttpPost("department-head/assign")]
    public async Task<IActionResult> CreateTeachingAssignment([FromBody] CreateTeachingAssignmentDto createDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        var assignment = await _teachingAssignmentService.CreateTeachingAssignmentAsync(userId, createDto);
        if (assignment == null)
        {
            return BadRequest(new { message = "Không thể tạo phân công giảng dạy. Bạn không phải trưởng khoa, giáo viên không thuộc khoa của bạn, hoặc phân công đã tồn tại." });
        }

        return CreatedAtAction(nameof(GetAllTeachingAssignments), assignment);
    }

    [HttpPut("department-head/{id}")]
    public async Task<IActionResult> UpdateTeachingAssignment(int id, [FromBody] UpdateTeachingAssignmentDto updateDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        var assignment = await _teachingAssignmentService.UpdateTeachingAssignmentAsync(id, userId, updateDto);
        if (assignment == null)
        {
            return BadRequest(new { message = "Không thể cập nhật phân công giảng dạy. Bạn không phải trưởng khoa hoặc giáo viên không thuộc khoa của bạn." });
        }

        return Ok(assignment);
    }

    [HttpDelete("department-head/{id}")]
    public async Task<IActionResult> DeleteTeachingAssignment(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        var result = await _teachingAssignmentService.DeleteTeachingAssignmentAsync(id, userId);
        if (!result)
        {
            return BadRequest(new { message = "Không thể xóa phân công giảng dạy. Bạn không có quyền hoặc phân công không tồn tại." });
        }

        return Ok(new { message = "Xóa phân công giảng dạy thành công" });
    }

    private ObjectResult Forbidden(object value)
    {
        return StatusCode(StatusCodes.Status403Forbidden, value);
    }
}
