using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Services;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "AcademicAffairs")]
public class ClassAssignmentController : ControllerBase
{
    private readonly IClassAssignmentService _classAssignmentService;

    public ClassAssignmentController(IClassAssignmentService classAssignmentService)
    {
        _classAssignmentService = classAssignmentService;
    }

    [HttpGet("class-info")]
    public async Task<IActionResult> GetClassInfo([FromQuery] string schoolYearId, [FromQuery] string gradeLevelId, [FromQuery] string classId)
    {
        var classInfo = await _classAssignmentService.GetClassInfoAsync(schoolYearId, gradeLevelId, classId);
        if (classInfo == null)
        {
            return NotFound(new { message = "Lớp học không tồn tại" });
        }
        return Ok(classInfo);
    }

    [HttpGet("students-in-class")]
    public async Task<IActionResult> GetStudentsInClass([FromQuery] string schoolYearId, [FromQuery] string gradeLevelId, [FromQuery] string classId)
    {
        var students = await _classAssignmentService.GetStudentsInClassAsync(schoolYearId, gradeLevelId, classId);
        return Ok(students);
    }

    [HttpGet("available-students")]
    public async Task<IActionResult> GetAvailableStudents([FromQuery] string schoolYearId, [FromQuery] string gradeLevelId)
    {
        var students = await _classAssignmentService.GetAvailableStudentsAsync(schoolYearId, gradeLevelId);
        return Ok(students);
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignStudentToClass([FromBody] AssignStudentToClassDto assignDto)
    {
        var result = await _classAssignmentService.AssignStudentToClassAsync(assignDto);
        if (!result)
        {
            return BadRequest(new { message = "Không thể phân công học sinh. Lớp có thể đã đầy hoặc học sinh đã được phân công vào lớp khác." });
        }
        return Ok(new { message = "Phân công học sinh thành công" });
    }

    [HttpPost("bulk-assign")]
    public async Task<IActionResult> BulkAssignStudents([FromBody] BulkAssignStudentsDto bulkAssignDto)
    {
        var result = await _classAssignmentService.BulkAssignStudentsToClassAsync(bulkAssignDto);
        if (!result)
        {
            return BadRequest(new { message = "Không thể phân công học sinh. Lớp có thể không đủ chỗ hoặc một số học sinh đã được phân công." });
        }
        return Ok(new { message = "Phân công hàng loạt thành công" });
    }

    [HttpPost("remove")]
    public async Task<IActionResult> RemoveStudentFromClass([FromBody] RemoveStudentFromClassDto removeDto)
    {
        var result = await _classAssignmentService.RemoveStudentFromClassAsync(removeDto);
        if (!result)
        {
            return NotFound(new { message = "Không tìm thấy phân công" });
        }
        return Ok(new { message = "Xóa phân công thành công" });
    }

    [HttpGet("student-history/{studentId}")]
    public async Task<IActionResult> GetStudentClassHistory(string studentId)
    {
        var history = await _classAssignmentService.GetStudentClassHistoryAsync(studentId);
        return Ok(history);
    }
}
