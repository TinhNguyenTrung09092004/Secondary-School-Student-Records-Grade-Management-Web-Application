using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API.Services;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AcademicRankingController : ControllerBase
{
    private readonly IAcademicRankingService _academicRankingService;

    public AcademicRankingController(IAcademicRankingService academicRankingService)
    {
        _academicRankingService = academicRankingService;
    }

    /// <summary>
    /// Calculate and update semester academic performance for a student
    /// </summary>
    [HttpPost("semester/calculate")]
    public async Task<IActionResult> CalculateSemesterAcademicPerformance(
        [FromQuery] string studentId,
        [FromQuery] string classId,
        [FromQuery] string schoolYearId,
        [FromQuery] string semesterId)
    {
        try
        {
            await _academicRankingService.UpdateSemesterAcademicPerformanceAsync(
                studentId, classId, schoolYearId, semesterId);

            return Ok(new { message = "Đã cập nhật học lực học kì thành công" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Calculate and update year academic performance and result title for a student
    /// </summary>
    [HttpPost("year/calculate")]
    public async Task<IActionResult> CalculateYearAcademicPerformance(
        [FromQuery] string studentId,
        [FromQuery] string classId,
        [FromQuery] string schoolYearId)
    {
        try
        {
            await _academicRankingService.UpdateYearAcademicPerformanceAsync(
                studentId, classId, schoolYearId);

            return Ok(new { message = "Đã cập nhật học lực và kết quả cả năm thành công" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get academic performance ranking for a semester (without updating)
    /// </summary>
    [HttpGet("semester/preview")]
    public async Task<IActionResult> PreviewSemesterAcademicPerformance(
        [FromQuery] string studentId,
        [FromQuery] string classId,
        [FromQuery] string schoolYearId,
        [FromQuery] string semesterId)
    {
        try
        {
            var academicPerformanceId = await _academicRankingService
                .CalculateSemesterAcademicPerformanceAsync(
                    studentId, classId, schoolYearId, semesterId);

            return Ok(new { academicPerformanceId });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get academic performance ranking and result title for a year (without updating)
    /// </summary>
    [HttpGet("year/preview")]
    public async Task<IActionResult> PreviewYearAcademicPerformance(
        [FromQuery] string studentId,
        [FromQuery] string classId,
        [FromQuery] string schoolYearId)
    {
        try
        {
            var academicPerformanceId = await _academicRankingService
                .CalculateYearAcademicPerformanceAsync(
                    studentId, classId, schoolYearId);

            var resultId = await _academicRankingService
                .CalculateYearResultTitleAsync(
                    studentId, classId, schoolYearId);

            return Ok(new
            {
                academicPerformanceId,
                resultId
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}