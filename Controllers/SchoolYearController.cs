using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Services;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SchoolYearController : ControllerBase
{
    private readonly ISchoolYearService _schoolYearService;

    public SchoolYearController(ISchoolYearService schoolYearService)
    {
        _schoolYearService = schoolYearService;
    }

    [HttpGet]
    [Authorize(Roles = "AcademicAffairs,SubjectTeacher,Principal")]
    public async Task<IActionResult> GetAll()
    {
        var schoolYears = await _schoolYearService.GetAllSchoolYearsAsync();
        return Ok(schoolYears);
    }

    [HttpGet("{schoolYearId}")]
    [Authorize(Roles = "AcademicAffairs,SubjectTeacher,Principal")]
    public async Task<IActionResult> GetById(string schoolYearId)
    {
        var schoolYear = await _schoolYearService.GetSchoolYearByIdAsync(schoolYearId);
        if (schoolYear == null)
        {
            return NotFound(new { message = "Năm học không tồn tại" });
        }
        return Ok(schoolYear);
    }

    [HttpPost]
    [Authorize(Roles = "AcademicAffairs")]
    public async Task<IActionResult> Create([FromBody] CreateSchoolYearDto createDto)
    {
        var schoolYear = await _schoolYearService.CreateSchoolYearAsync(createDto);
        if (schoolYear == null)
        {
            return BadRequest(new { message = "Mã năm học đã tồn tại" });
        }
        return CreatedAtAction(nameof(GetById), new { schoolYearId = schoolYear.SchoolYearId }, schoolYear);
    }

    [HttpPut("{schoolYearId}")]
    [Authorize(Roles = "AcademicAffairs")]
    public async Task<IActionResult> Update(string schoolYearId, [FromBody] UpdateSchoolYearDto updateDto)
    {
        var schoolYear = await _schoolYearService.UpdateSchoolYearAsync(schoolYearId, updateDto);
        if (schoolYear == null)
        {
            return BadRequest(new { message = "Năm học không tồn tại hoặc mã năm học mới đã tồn tại" });
        }
        return Ok(schoolYear);
    }

    [HttpDelete("{schoolYearId}")]
    [Authorize(Roles = "AcademicAffairs")]
    public async Task<IActionResult> Delete(string schoolYearId)
    {
        var result = await _schoolYearService.DeleteSchoolYearAsync(schoolYearId);
        if (!result)
        {
            return BadRequest(new { message = "Không thể xóa năm học. Có thể năm học này đang có dữ liệu liên quan (lớp, phân công, điểm, kết quả học tập)." });
        }
        return Ok(new { message = "Năm học đã được xóa thành công" });
    }
}
