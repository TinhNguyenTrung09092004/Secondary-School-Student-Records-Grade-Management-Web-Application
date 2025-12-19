using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Repositories;
using API.DTOs;
using API.Models;
using Microsoft.EntityFrameworkCore;
using API.Data;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GradeTypeController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public GradeTypeController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllGradeTypes()
    {
        var gradeTypes = await _context.GradeTypes
            .Select(gt => new GradeTypeDto
            {
                GradeTypeId = gt.GradeTypeId,
                GradeTypeName = gt.GradeTypeName,
                Coefficient = gt.Coefficient
            })
            .ToListAsync();
        return Ok(gradeTypes);
    }

    [HttpPost]
    [Authorize(Roles = "Principal")]
    public async Task<IActionResult> CreateGradeType([FromBody] CreateGradeTypeDto createDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (await _context.GradeTypes.AnyAsync(gt => gt.GradeTypeId == createDto.GradeTypeId))
            return BadRequest(new { message = "Mã loại điểm đã tồn tại" });

        var gradeType = new GradeType
        {
            GradeTypeId = createDto.GradeTypeId,
            GradeTypeName = createDto.GradeTypeName,
            Coefficient = createDto.Coefficient
        };

        _context.GradeTypes.Add(gradeType);
        await _context.SaveChangesAsync();

        return Ok(new GradeTypeDto
        {
            GradeTypeId = gradeType.GradeTypeId,
            GradeTypeName = gradeType.GradeTypeName,
            Coefficient = gradeType.Coefficient
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Principal")]
    public async Task<IActionResult> UpdateGradeType(string id, [FromBody] UpdateGradeTypeDto updateDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var gradeType = await _context.GradeTypes.FindAsync(id);
        if (gradeType == null) return NotFound(new { message = "Không tìm thấy loại điểm" });

        gradeType.GradeTypeId = updateDto.GradeTypeId;
        gradeType.GradeTypeName = updateDto.GradeTypeName;
        gradeType.Coefficient = updateDto.Coefficient;

        await _context.SaveChangesAsync();

        return Ok(new GradeTypeDto
        {
            GradeTypeId = gradeType.GradeTypeId,
            GradeTypeName = gradeType.GradeTypeName,
            Coefficient = gradeType.Coefficient
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Principal")]
    public async Task<IActionResult> DeleteGradeType(string id)
    {
        var gradeType = await _context.GradeTypes
            .Include(gt => gt.Grades)
            .FirstOrDefaultAsync(gt => gt.GradeTypeId == id);

        if (gradeType == null) return NotFound(new { message = "Không tìm thấy loại điểm" });

        if (gradeType.Grades.Any())
            return BadRequest(new { message = "Không thể xóa loại điểm đang được sử dụng" });

        _context.GradeTypes.Remove(gradeType);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
