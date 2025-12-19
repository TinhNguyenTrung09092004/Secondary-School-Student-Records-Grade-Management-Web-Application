using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API.DTOs;
using API.Services;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FaceRecognitionController : ControllerBase
{
    private readonly IFaceRecognitionService _faceRecognitionService;

    public FaceRecognitionController(IFaceRecognitionService faceRecognitionService)
    {
        _faceRecognitionService = faceRecognitionService;
    }

    [HttpPost("enroll")]
    [Authorize]
    public async Task<IActionResult> EnrollFace([FromBody] EnrollFaceDto enrollDto)
    {
        try
        {
            var result = await _faceRecognitionService.EnrollFaceAsync(enrollDto);

            if (!result)
            {
                return BadRequest(new { message = "Không thể đăng ký khuôn mặt. Vui lòng thử lại." });
            }

            return Ok(new { message = "Đăng ký khuôn mặt thành công" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginWithFace([FromBody] FaceLoginDto faceLoginDto)
    {
        try
        {
            var result = await _faceRecognitionService.AuthenticateWithFaceAsync(faceLoginDto);

            if (result == null)
            {
                return Unauthorized(new { message = "Không nhận diện được khuôn mặt" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("status/{email}")]
    [Authorize]
    public async Task<IActionResult> GetEnrollmentStatus(string email)
    {
        try
        {
            var status = await _faceRecognitionService.GetEnrollmentStatusAsync(email);
            return Ok(status);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}