using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class BackupController : ControllerBase
{
    private readonly IBackupService _backupService;

    public BackupController(IBackupService backupService)
    {
        _backupService = backupService;
    }

    [HttpGet("tables")]
    public async Task<IActionResult> GetTables()
    {
        var tables = await _backupService.GetAllTablesAsync();
        return Ok(tables);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateBackup([FromBody] ManualBackupRequest request)
    {
        if (request.Tables == null || request.Tables.Count == 0)
        {
            return BadRequest("Please select at least one table");
        }

        try
        {
            var fileName = await _backupService.CreateBackupAsync(request.Tables);
            return Ok(new { message = "Backup created successfully", fileName });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Backup failed: {ex.Message}" });
        }
    }

    [HttpGet("files")]
    public async Task<IActionResult> GetBackupFiles()
    {
        var files = await _backupService.GetBackupFilesAsync();
        return Ok(files);
    }

    [HttpGet("download/{fileName}")]
    public async Task<IActionResult> DownloadBackup(string fileName)
    {
        try
        {
            var filePath = await _backupService.GetBackupFilePathAsync(fileName);
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/sql", fileName);
        }
        catch (FileNotFoundException)
        {
            return NotFound("Backup file not found");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("schedule")]
    public async Task<IActionResult> GetSchedule()
    {
        var schedule = await _backupService.GetBackupScheduleAsync();
        return Ok(schedule);
    }

    [HttpPost("schedule")]
    public async Task<IActionResult> UpdateSchedule([FromBody] BackupScheduleDto dto)
    {
        try
        {
            await _backupService.UpdateBackupScheduleAsync(dto);
            return Ok(new { message = "Backup schedule updated successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("restore/{fileName}")]
    public async Task<IActionResult> RestoreBackup(string fileName)
    {
        try
        {
            await _backupService.RestoreBackupAsync(fileName);
            return Ok(new { message = "Backup restored successfully" });
        }
        catch (FileNotFoundException)
        {
            return NotFound("Backup file not found");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Restore failed: {ex.Message}" });
        }
    }
}
