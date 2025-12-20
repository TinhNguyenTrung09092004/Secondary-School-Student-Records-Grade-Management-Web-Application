using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API.Services;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentSubjectResultController : ControllerBase
{
    private readonly IStudentSubjectResultService _studentSubjectResultService;

    public StudentSubjectResultController(IStudentSubjectResultService studentSubjectResultService)
    {
        _studentSubjectResultService = studentSubjectResultService;
    }

    /// <summary>
    /// Calculate and save complete semester data for a student
    /// (subject results + semester summary)
    /// </summary>
    [HttpPost("calculate/semester")]
    public async Task<IActionResult> CalculateCompleteSemester(
        [FromQuery] string studentId,
        [FromQuery] string classId,
        [FromQuery] string schoolYearId,
        [FromQuery] string semesterId)
    {
        try
        {
            await _studentSubjectResultService.CalculateAndSaveCompleteSemesterAsync(
                studentId, classId, schoolYearId, semesterId);

            return Ok(new { message = "Đã tính toán và lưu kết quả học kì thành công" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Calculate and save complete year data for a student
    /// (both semesters + year summary)
    /// </summary>
    [HttpPost("calculate/year")]
    public async Task<IActionResult> CalculateCompleteYear(
        [FromQuery] string studentId,
        [FromQuery] string classId,
        [FromQuery] string schoolYearId)
    {
        try
        {
            await _studentSubjectResultService.CalculateAndSaveCompleteYearAsync(
                studentId, classId, schoolYearId);

            return Ok(new { message = "Đã tính toán và lưu kết quả cả năm thành công" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Calculate and save complete semester data for all students in a class
    /// </summary>
    [HttpPost("calculate/class/semester")]
    public async Task<IActionResult> CalculateClassSemester(
        [FromQuery] string classId,
        [FromQuery] string schoolYearId,
        [FromQuery] string semesterId)
    {
        try
        {
            await _studentSubjectResultService.CalculateAndSaveClassSemesterAsync(
                classId, schoolYearId, semesterId);

            return Ok(new { message = "Đã tính toán và lưu kết quả học kì cho cả lớp thành công" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Calculate and save complete year data for all students in a class
    /// </summary>
    [HttpPost("calculate/class/year")]
    public async Task<IActionResult> CalculateClassYear(
        [FromQuery] string classId,
        [FromQuery] string schoolYearId)
    {
        try
        {
            await _studentSubjectResultService.CalculateAndSaveClassYearAsync(
                classId, schoolYearId);

            return Ok(new { message = "Đã tính toán và lưu kết quả cả năm cho cả lớp thành công" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
