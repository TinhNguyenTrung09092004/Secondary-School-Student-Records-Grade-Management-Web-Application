using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class AcademicRankingService : IAcademicRankingService
{
    private readonly ApplicationDbContext _context;

    public AcademicRankingService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> CalculateSemesterAcademicPerformanceAsync(
        string studentId, string classId, string schoolYearId, string semesterId)
    {
        // Get all subject results for this student in this semester
        var subjectResults = await _context.StudentSubjectResults
            .Where(ssr => ssr.StudentId == studentId &&
                         ssr.ClassId == classId &&
                         ssr.SchoolYearId == schoolYearId &&
                         ssr.SemesterId == semesterId)
            .ToListAsync();

        // Get all grades (for comment-based subjects)
        var grades = await _context.Grades
            .Where(g => g.StudentId == studentId &&
                       g.ClassId == classId &&
                       g.SchoolYearId == schoolYearId &&
                       g.SemesterId == semesterId &&
                       g.IsComment)
            .ToListAsync();

        return CalculateAcademicPerformanceRanking(subjectResults, grades);
    }

    public async Task<string> CalculateYearAcademicPerformanceAsync(
        string studentId, string classId, string schoolYearId)
    {
        // For year calculation, we need to get average of both semesters
        // Get subject results for both semesters
        var subjectResults = await _context.StudentSubjectResults
            .Where(ssr => ssr.StudentId == studentId &&
                         ssr.ClassId == classId &&
                         ssr.SchoolYearId == schoolYearId)
            .GroupBy(ssr => ssr.SubjectId)
            .Select(g => new StudentSubjectResult
            {
                StudentId = studentId,
                ClassId = classId,
                SchoolYearId = schoolYearId,
                SubjectId = g.Key,
                SemesterId = "", // Not used for year average
                // Calculate year average as (sem1 + sem2*2) / 3
                AverageSemester = g.Average(s => s.AverageSemester)
            })
            .ToListAsync();

        // Get comment-based grades (check if any subject has "Chưa đạt" in either semester)
        var commentGrades = await _context.Grades
            .Where(g => g.StudentId == studentId &&
                       g.ClassId == classId &&
                       g.SchoolYearId == schoolYearId &&
                       g.IsComment)
            .ToListAsync();

        return CalculateAcademicPerformanceRanking(subjectResults, commentGrades);
    }

    public async Task<string> CalculateYearResultTitleAsync(
        string studentId, string classId, string schoolYearId)
    {
        // Get year result
        var yearResult = await _context.StudentYearResults
            .FirstOrDefaultAsync(syr => syr.StudentId == studentId &&
                                       syr.ClassId == classId &&
                                       syr.SchoolYearId == schoolYearId);

        if (yearResult == null)
            return ""; // No result yet

        // Get academic performance and conduct IDs
        var academicPerformance = await _context.AcademicPerformances
            .FirstOrDefaultAsync(ap => ap.AcademicPerformanceId == yearResult.AcademicPerformanceId);

        var conduct = await _context.Conducts
            .FirstOrDefaultAsync(c => c.ConductId == yearResult.ConductId);

        if (academicPerformance == null || conduct == null)
            return "";

        // Get subject results for year to check for 9.0+ subjects
        var subjectResults = await _context.StudentSubjectResults
            .Where(ssr => ssr.StudentId == studentId &&
                         ssr.ClassId == classId &&
                         ssr.SchoolYearId == schoolYearId)
            .GroupBy(ssr => ssr.SubjectId)
            .Select(g => g.Average(s => s.AverageSemester))
            .ToListAsync();

        int subjectsAbove9 = subjectResults.Count(avg => avg >= 9.0m);

        // Apply rules:
        // "Học sinh Xuất sắc": Conduct Tốt, Academic Tốt, at least 6 subjects >= 9.0
        if (conduct.ConductName == "Tốt" &&
            academicPerformance.AcademicPerformanceName == "Tốt" &&
            subjectsAbove9 >= 6)
        {
            var excellentResult = await _context.Results
                .FirstOrDefaultAsync(r => r.ResultName == "Xuất sắc");
            return excellentResult?.ResultId ?? "";
        }

        // "Học sinh Giỏi": Conduct Tốt, Academic Tốt
        if (conduct.ConductName == "Tốt" &&
            academicPerformance.AcademicPerformanceName == "Tốt")
        {
            var goodResult = await _context.Results
                .FirstOrDefaultAsync(r => r.ResultName == "Giỏi");
            return goodResult?.ResultId ?? "";
        }

        // "Học sinh tiên tiến": Conduct >= Khá, Academic >= Khá
        if ((conduct.ConductName == "Tốt" || conduct.ConductName == "Khá") &&
            (academicPerformance.AcademicPerformanceName == "Tốt" ||
             academicPerformance.AcademicPerformanceName == "Khá"))
        {
            var advancedResult = await _context.Results
                .FirstOrDefaultAsync(r => r.ResultName == "Tiên tiến");
            return advancedResult?.ResultId ?? "";
        }

        return ""; // No title
    }

    public async Task UpdateSemesterAcademicPerformanceAsync(
        string studentId, string classId, string schoolYearId, string semesterId)
    {
        var academicPerformanceId = await CalculateSemesterAcademicPerformanceAsync(
            studentId, classId, schoolYearId, semesterId);

        var semesterResult = await _context.StudentSemesterResults
            .FirstOrDefaultAsync(ssr => ssr.StudentId == studentId &&
                                       ssr.ClassId == classId &&
                                       ssr.SchoolYearId == schoolYearId &&
                                       ssr.SemesterId == semesterId);

        if (semesterResult != null)
        {
            semesterResult.AcademicPerformanceId = academicPerformanceId;
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateYearAcademicPerformanceAsync(
        string studentId, string classId, string schoolYearId)
    {
        var academicPerformanceId = await CalculateYearAcademicPerformanceAsync(
            studentId, classId, schoolYearId);

        var yearResult = await _context.StudentYearResults
            .FirstOrDefaultAsync(syr => syr.StudentId == studentId &&
                                       syr.ClassId == classId &&
                                       syr.SchoolYearId == schoolYearId);

        if (yearResult != null)
        {
            yearResult.AcademicPerformanceId = academicPerformanceId;
            await _context.SaveChangesAsync();

            // Now calculate result title after academic performance is set
            var resultId = await CalculateYearResultTitleAsync(
                studentId, classId, schoolYearId);

            yearResult.ResultId = resultId;
            await _context.SaveChangesAsync();
        }
    }

    private string CalculateAcademicPerformanceRanking(
        List<StudentSubjectResult> subjectResults,
        List<Grade> commentGrades)
    {
        // Check comment-based subjects
        int commentFailCount = commentGrades.Count(g => g.Comment == "Chưa đạt");
        int commentPassCount = commentGrades.Count(g => g.Comment == "Đạt");
        bool allCommentsPass = commentFailCount == 0;
        bool atMostOneCommentFail = commentFailCount <= 1;

        // Get score-based subject averages
        var scoreAverages = subjectResults.Select(sr => sr.AverageSemester).ToList();
        int totalScoreSubjects = scoreAverages.Count;

        if (totalScoreSubjects == 0 && commentGrades.Count == 0)
            return ""; // No data yet

        // Count subjects by score thresholds
        int subjectsAbove8 = scoreAverages.Count(avg => avg >= 8.0m);
        int subjectsAbove6_5 = scoreAverages.Count(avg => avg >= 6.5m);
        int subjectsAbove5 = scoreAverages.Count(avg => avg >= 5.0m);
        int subjectsBelow3_5 = scoreAverages.Count(avg => avg < 3.5m);

        bool allScoresAbove6_5 = totalScoreSubjects == 0 || scoreAverages.All(avg => avg >= 6.5m);
        bool allScoresAbove5 = totalScoreSubjects == 0 || scoreAverages.All(avg => avg >= 5.0m);

        // Apply ranking rules (with partial data support):
        // Note: "at least 6 subjects" is checked, but ranking can be calculated with fewer subjects

        // a) Tốt: All comments Đạt, all scores >= 6.5, at least 6 scores >= 8.0 (if 6+ subjects exist)
        if (allCommentsPass && allScoresAbove6_5 &&
            (totalScoreSubjects < 6 || subjectsAbove8 >= 6))
        {
            return GetAcademicPerformanceId("Tốt").Result;
        }

        // b) Khá: All comments Đạt, all scores >= 5.0, at least 6 scores >= 6.5 (if 6+ subjects exist)
        if (allCommentsPass && allScoresAbove5 &&
            (totalScoreSubjects < 6 || subjectsAbove6_5 >= 6))
        {
            return GetAcademicPerformanceId("Khá").Result;
        }

        // c) Đạt: At most 1 comment fail, at least 6 scores >= 5.0 (or all if < 6), no score < 3.5
        if (atMostOneCommentFail &&
            (totalScoreSubjects < 6 ? subjectsAbove5 == totalScoreSubjects : subjectsAbove5 >= 6) &&
            subjectsBelow3_5 == 0)
        {
            return GetAcademicPerformanceId("Đạt").Result;
        }

        // d) Chưa đạt: All other cases
        return GetAcademicPerformanceId("Chưa đạt").Result;
    }

    private async Task<string> GetAcademicPerformanceId(string name)
    {
        var academicPerformance = await _context.AcademicPerformances
            .FirstOrDefaultAsync(ap => ap.AcademicPerformanceName == name);
        return academicPerformance?.AcademicPerformanceId ?? "";
    }
}