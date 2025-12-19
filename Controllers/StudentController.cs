using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Services;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "AcademicAffairs")]
public class StudentController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var students = await _studentService.GetAllStudentsAsync();
        return Ok(students);
    }

    [HttpGet("{studentId}")]
    public async Task<IActionResult> GetById(string studentId)
    {
        var student = await _studentService.GetStudentByIdAsync(studentId);
        if (student == null)
        {
            return NotFound(new { message = "Học sinh không tồn tại" });
        }
        return Ok(student);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStudentDto createDto)
    {
        var student = await _studentService.CreateStudentAsync(createDto);
        if (student == null)
        {
            return BadRequest(new { message = "Mã học sinh hoặc email đã tồn tại" });
        }
        return CreatedAtAction(nameof(GetById), new { studentId = student.StudentId }, student);
    }

    [HttpPut("{studentId}")]
    public async Task<IActionResult> Update(string studentId, [FromBody] UpdateStudentDto updateDto)
    {
        var student = await _studentService.UpdateStudentAsync(studentId, updateDto);
        if (student == null)
        {
            return BadRequest(new { message = "Không thể cập nhật học sinh. Email có thể đã tồn tại hoặc học sinh không tồn tại" });
        }
        return Ok(student);
    }

    [HttpDelete("{studentId}")]
    public async Task<IActionResult> Delete(string studentId)
    {
        var result = await _studentService.DeleteStudentAsync(studentId);
        if (!result)
        {
            return BadRequest(new { message = "Không thể xóa học sinh. Có thể học sinh này đang có dữ liệu liên quan (lớp, điểm, kết quả học tập)." });
        }
        return Ok(new { message = "Học sinh đã được xóa thành công" });
    }
}
