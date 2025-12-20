using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Services;
using System.Security.Claims;
using API.Repositories;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SubjectTeacher")]
public class GradeController : ControllerBase
{
    private readonly IGradeService _gradeService;
    private readonly ITeacherRepository _teacherRepository;

    public GradeController(IGradeService gradeService, ITeacherRepository teacherRepository)
    {
        _gradeService = gradeService;
        _teacherRepository = teacherRepository;
    }

    private async Task<string?> GetTeacherIdFromUserAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }

        var teacher = await _teacherRepository.GetByUserIdAsync(userId);
        return teacher?.TeacherId;
    }

    [HttpGet("teacher-classes")]
    public async Task<IActionResult> GetTeacherClasses()
    {
        var teacherId = await GetTeacherIdFromUserAsync();
        if (teacherId == null)
        {
            return Unauthorized(new { message = "Không thể xác định giáo viên" });
        }

        var classes = await _gradeService.GetTeacherClassSubjectsAsync(teacherId);
        return Ok(classes);
    }

    [HttpGet("students")]
    public async Task<IActionResult> GetStudentGrades(
        [FromQuery] string classId,
        [FromQuery] string subjectId,
        [FromQuery] string semesterId,
        [FromQuery] string schoolYearId)
    {
        if (string.IsNullOrEmpty(classId) || string.IsNullOrEmpty(subjectId) ||
            string.IsNullOrEmpty(semesterId) || string.IsNullOrEmpty(schoolYearId))
        {
            return BadRequest(new { message = "Thiếu thông tin cần thiết" });
        }

        var studentGrades = await _gradeService.GetStudentGradesForClassSubjectAsync(
            classId, subjectId, semesterId, schoolYearId);
        return Ok(studentGrades);
    }

    [HttpGet("view")]
    public async Task<IActionResult> GetStudentGradesView(
        [FromQuery] string classId,
        [FromQuery] string subjectId,
        [FromQuery] string schoolYearId)
    {
        if (string.IsNullOrEmpty(classId) || string.IsNullOrEmpty(subjectId) || string.IsNullOrEmpty(schoolYearId))
        {
            return BadRequest(new { message = "Thiếu thông tin cần thiết" });
        }

        try
        {
            var studentGradesView = await _gradeService.GetStudentGradesViewAsync(classId, subjectId, schoolYearId);
            return Ok(studentGradesView);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi khi tải điểm: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateGrade([FromBody] CreateGradeDto createGradeDto)
    {
        try
        {
            var grade = await _gradeService.CreateGradeAsync(createGradeDto);
            return CreatedAtAction(nameof(CreateGrade), new { id = grade.RowNumber }, grade);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi khi tạo điểm: " + ex.Message });
        }
    }

    [HttpPut("{rowNumber}")]
    public async Task<IActionResult> UpdateGrade(int rowNumber, [FromBody] UpdateGradeDto updateGradeDto)
    {
        try
        {
            var grade = await _gradeService.UpdateGradeAsync(rowNumber, updateGradeDto);
            return Ok(grade);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi khi cập nhật điểm: " + ex.Message });
        }
    }

    [HttpDelete("{rowNumber}")]
    public async Task<IActionResult> DeleteGrade(int rowNumber)
    {
        try
        {
            await _gradeService.DeleteGradeAsync(rowNumber);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi khi xóa điểm: " + ex.Message });
        }
    }

    [HttpPost("import-excel")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ImportGradesFromExcel(
        IFormFile file,
        [FromForm] string classId,
        [FromForm] string subjectId,
        [FromForm] string semesterId,
        [FromForm] string schoolYearId,
        [FromForm] bool isComment)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "Vui lòng chọn file Excel" });
        }

        if (string.IsNullOrEmpty(classId) || string.IsNullOrEmpty(subjectId) ||
            string.IsNullOrEmpty(semesterId) || string.IsNullOrEmpty(schoolYearId))
        {
            return BadRequest(new { message = "Thiếu thông tin cần thiết" });
        }

        try
        {
            var result = await _gradeService.ImportGradesFromExcelAsync(
                file, classId, subjectId, semesterId, schoolYearId, isComment);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi khi import điểm: " + ex.Message });
        }
    }

    [HttpGet("export")]
    public async Task<IActionResult> ExportGrades(
        [FromQuery] string classId,
        [FromQuery] string subjectId,
        [FromQuery] string semesterId,
        [FromQuery] string schoolYearId,
        [FromQuery] string format)
    {
        if (string.IsNullOrEmpty(classId) || string.IsNullOrEmpty(subjectId) ||
            string.IsNullOrEmpty(semesterId) || string.IsNullOrEmpty(schoolYearId) ||
            string.IsNullOrEmpty(format))
        {
            return BadRequest(new { message = "Thiếu thông tin cần thiết" });
        }

        if (format != "excel" && format != "word" && format != "pdf")
        {
            return BadRequest(new { message = "Định dạng không hợp lệ. Chỉ hỗ trợ: excel, word, pdf" });
        }

        try
        {
            var fileData = await _gradeService.ExportGradesAsync(
                classId, subjectId, semesterId, schoolYearId, format);

            string contentType = format switch
            {
                "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "word" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "pdf" => "application/pdf",
                _ => "application/octet-stream"
            };

            string extension = format switch
            {
                "excel" => "xlsx",
                "word" => "docx",
                "pdf" => "pdf",
                _ => "dat"
            };

            return File(fileData, contentType, $"Diem_{classId}_{subjectId}_{semesterId}.{extension}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi khi export điểm: " + ex.Message });
        }
    }
}
