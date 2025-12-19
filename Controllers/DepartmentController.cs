using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Services;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepartmentController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    [Authorize(Roles = "Principal,SubjectTeacher")]
    public async Task<IActionResult> GetAllDepartments()
    {
        var departments = await _departmentService.GetAllDepartmentsAsync();
        return Ok(departments);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Principal,SubjectTeacher")]
    public async Task<IActionResult> GetDepartmentById(string id)
    {
        var department = await _departmentService.GetDepartmentByIdAsync(id);
        if (department == null)
        {
            return NotFound(new { message = "Không tìm thấy tổ bộ môn" });
        }
        return Ok(department);
    }

    [HttpPost]
    [Authorize(Roles = "Principal")]
    public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentDto createDto)
    {
        var department = await _departmentService.CreateDepartmentAsync(createDto);
        if (department == null)
        {
            return BadRequest(new { message = "Không thể tạo tổ bộ môn. Mã tổ bộ môn đã tồn tại hoặc giáo viên trưởng khoa không hợp lệ." });
        }
        return CreatedAtAction(nameof(GetDepartmentById), new { id = department.DepartmentId }, department);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Principal")]
    public async Task<IActionResult> UpdateDepartment(string id, [FromBody] UpdateDepartmentDto updateDto)
    {
        var department = await _departmentService.UpdateDepartmentAsync(id, updateDto);
        if (department == null)
        {
            return BadRequest(new { message = "Không thể cập nhật tổ bộ môn. Tổ bộ môn không tồn tại hoặc giáo viên trưởng khoa không hợp lệ." });
        }
        return Ok(department);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Principal")]
    public async Task<IActionResult> DeleteDepartment(string id)
    {
        var result = await _departmentService.DeleteDepartmentAsync(id);
        if (!result)
        {
            return BadRequest(new { message = "Không thể xóa tổ bộ môn. Tổ bộ môn không tồn tại hoặc còn giáo viên được phân công." });
        }
        return Ok(new { message = "Xóa tổ bộ môn thành công" });
    }
}
