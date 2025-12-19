using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.DTOs;
using API.Services;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Principal,Admin")]
public class PrincipalDashboardController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IAcademicRankingService _rankingService;

    public PrincipalDashboardController(
        ApplicationDbContext context,
        IAcademicRankingService rankingService)
    {
        _context = context;
        _rankingService = rankingService;
    }

    /// <summary>
    /// Get student results by grade level for principal dashboard
    /// </summary>
    [HttpGet("grade-results/{schoolYearId}")]
    public async Task<IActionResult> GetGradeLevelResults(string schoolYearId)
    {
        try
        {
            var gradeLevels = await _context.GradeLevels
                .OrderBy(gl => gl.GradeLevelId)
                .ToListAsync();

            var results = new List<GradeLevelResultsDto>();

            foreach (var gradeLevel in gradeLevels)
            {
                var classes = await _context.Classes
                    .Where(c => c.GradeLevelId == gradeLevel.GradeLevelId &&
                               c.SchoolYearId == schoolYearId)
                    .OrderBy(c => c.ClassName)
                    .ToListAsync();

                var gradeLevelDto = new GradeLevelResultsDto
                {
                    GradeLevelId = gradeLevel.GradeLevelId,
                    GradeLevelName = gradeLevel.GradeLevelName,
                    Classes = new List<ClassResultsDto>()
                };

                foreach (var classItem in classes)
                {
                    var classDto = new ClassResultsDto
                    {
                        ClassId = classItem.ClassId,
                        ClassName = classItem.ClassName,
                        Students = await GetStudentResults(classItem.ClassId, schoolYearId)
                    };

                    gradeLevelDto.Classes.Add(classDto);
                }

                results.Add(gradeLevelDto);
            }

            return Ok(results);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Recalculate all rankings for a school year
    /// </summary>
    [HttpPost("recalculate-all/{schoolYearId}")]
    public async Task<IActionResult> RecalculateAllRankings(string schoolYearId)
    {
        try
        {
            var students = await _context.ClassAssignments
                .Where(ca => ca.Class.SchoolYearId == schoolYearId)
                .Select(ca => new { ca.StudentId, ca.ClassId })
                .ToListAsync();

            var semesters = await _context.Semesters.ToListAsync();

            foreach (var student in students)
            {
                // Calculate semester rankings
                foreach (var semester in semesters)
                {
                    await _rankingService.UpdateSemesterAcademicPerformanceAsync(
                        student.StudentId, student.ClassId, schoolYearId, semester.SemesterId);
                }

                // Calculate year ranking
                await _rankingService.UpdateYearAcademicPerformanceAsync(
                    student.StudentId, student.ClassId, schoolYearId);
            }

            return Ok(new { message = $"Đã tính toán lại học lực cho {students.Count} học sinh" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private async Task<List<StudentResultDto>> GetStudentResults(string classId, string schoolYearId)
    {
        var students = await _context.ClassAssignments
            .Where(ca => ca.ClassId == classId)
            .Include(ca => ca.Student)
            .OrderBy(ca => ca.Student.FullName)
            .Select(ca => ca.Student)
            .ToListAsync();

        var results = new List<StudentResultDto>();

        foreach (var student in students)
        {
            var semester1Result = await _context.StudentSemesterResults
                .Include(ssr => ssr.AcademicPerformance)
                .Include(ssr => ssr.Conduct)
                .FirstOrDefaultAsync(ssr => ssr.StudentId == student.StudentId &&
                                           ssr.ClassId == classId &&
                                           ssr.SchoolYearId == schoolYearId &&
                                           ssr.SemesterId == "HK1");

            var semester2Result = await _context.StudentSemesterResults
                .Include(ssr => ssr.AcademicPerformance)
                .Include(ssr => ssr.Conduct)
                .FirstOrDefaultAsync(ssr => ssr.StudentId == student.StudentId &&
                                           ssr.ClassId == classId &&
                                           ssr.SchoolYearId == schoolYearId &&
                                           ssr.SemesterId == "HK2");

            var yearResult = await _context.StudentYearResults
                .Include(syr => syr.AcademicPerformance)
                .Include(syr => syr.Conduct)
                .Include(syr => syr.Result)
                .FirstOrDefaultAsync(syr => syr.StudentId == student.StudentId &&
                                           syr.ClassId == classId &&
                                           syr.SchoolYearId == schoolYearId);

            var subjectResults = await GetSubjectResults(student.StudentId, classId, schoolYearId);

            results.Add(new StudentResultDto
            {
                StudentId = student.StudentId,
                StudentName = student.FullName,

                Semester1AcademicPerformance = semester1Result?.AcademicPerformance?.AcademicPerformanceName,
                Semester1Conduct = semester1Result?.Conduct?.ConductName,
                Semester1Average = semester1Result?.AverageSemester,

                Semester2AcademicPerformance = semester2Result?.AcademicPerformance?.AcademicPerformanceName,
                Semester2Conduct = semester2Result?.Conduct?.ConductName,
                Semester2Average = semester2Result?.AverageSemester,

                YearAcademicPerformance = yearResult?.AcademicPerformance?.AcademicPerformanceName,
                YearConduct = yearResult?.Conduct?.ConductName,
                YearResult = yearResult?.Result?.ResultName,
                YearAverage = yearResult?.AverageYear,

                SubjectResults = subjectResults
            });
        }

        return results;
    }

    private async Task<List<SubjectResultDto>> GetSubjectResults(
        string studentId, string classId, string schoolYearId)
    {
        var subjectResults = await _context.StudentSubjectResults
            .Where(ssr => ssr.StudentId == studentId &&
                         ssr.ClassId == classId &&
                         ssr.SchoolYearId == schoolYearId)
            .Include(ssr => ssr.Subject)
            .GroupBy(ssr => new { ssr.SubjectId, ssr.Subject.SubjectName })
            .Select(g => new SubjectResultDto
            {
                SubjectId = g.Key.SubjectId,
                SubjectName = g.Key.SubjectName,
                Semester1Average = g.Where(s => s.SemesterId == "HK1")
                                    .Select(s => (decimal?)s.AverageSemester)
                                    .FirstOrDefault(),
                Semester2Average = g.Where(s => s.SemesterId == "HK2")
                                    .Select(s => (decimal?)s.AverageSemester)
                                    .FirstOrDefault(),
                YearAverage = g.Average(s => s.AverageSemester)
            })
            .ToListAsync();

        return subjectResults;
    }
}
