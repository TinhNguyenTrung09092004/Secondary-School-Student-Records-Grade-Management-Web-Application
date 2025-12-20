using API.Data;
using API.Models;
using API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class StudentSubjectResultService : IStudentSubjectResultService
{
    private readonly ApplicationDbContext _context;
    private readonly IStudentSubjectResultRepository _studentSubjectResultRepository;
    private readonly IStudentSemesterResultRepository _studentSemesterResultRepository;
    private readonly IStudentYearResultRepository _studentYearResultRepository;

    public StudentSubjectResultService(
        ApplicationDbContext context,
        IStudentSubjectResultRepository studentSubjectResultRepository,
        IStudentSemesterResultRepository studentSemesterResultRepository,
        IStudentYearResultRepository studentYearResultRepository)
    {
        _context = context;
        _studentSubjectResultRepository = studentSubjectResultRepository;
        _studentSemesterResultRepository = studentSemesterResultRepository;
        _studentYearResultRepository = studentYearResultRepository;
    }

    public async Task CalculateAndSaveSubjectResultAsync(
        string studentId, string subjectId, string classId,
        string schoolYearId, string semesterId)
    {
        // Get all grades for this student, subject, and semester
        var allGrades = await _context.Grades
            .Include(g => g.GradeType)
            .Where(g => g.StudentId == studentId &&
                       g.SubjectId == subjectId &&
                       g.ClassId == classId &&
                       g.SchoolYearId == schoolYearId &&
                       g.SemesterId == semesterId)
            .ToListAsync();

        if (!allGrades.Any())
        {
            // No grades found for this subject
            return;
        }

        // Separate numeric and comment-based grades
        var numericGrades = allGrades.Where(g => !g.IsComment).ToList();
        var commentGrades = allGrades.Where(g => g.IsComment).ToList();

        decimal? semesterAverage = null;
        string? commentResult = null;

        // Check if this is a comment-based subject
        if (commentGrades.Any() && !numericGrades.Any())
        {
            // Comment-based subject: check if all comments are "Pass" or "Đạt"
            bool allPass = commentGrades.All(g =>
                g.Comment == "Pass" ||
                g.Comment == "Đạt");
            commentResult = allPass ? "Đạt" : "Chưa đạt";
        }
        else if (numericGrades.Any())
        {
            // Numeric subject: calculate weighted average
            decimal totalWeightedScore = 0;
            int totalCoefficient = 0;

            foreach (var grade in numericGrades)
            {
                if (grade.Score.HasValue)
                {
                    totalWeightedScore += grade.Score.Value * grade.GradeType.Coefficient;
                    totalCoefficient += grade.GradeType.Coefficient;
                }
            }

            if (totalCoefficient > 0)
            {
                semesterAverage = totalWeightedScore / totalCoefficient;
            }
        }
        else
        {
            // No valid grades
            return;
        }

        // Check if result already exists
        var existingResult = await _studentSubjectResultRepository
            .GetByIdAsync(studentId, classId, schoolYearId, subjectId, semesterId);

        if (existingResult != null)
        {
            // Update existing result
            existingResult.AverageSemester = semesterAverage;
            existingResult.CommentResult = commentResult;
            await _studentSubjectResultRepository.UpdateAsync(existingResult);
        }
        else
        {
            // Create new result
            var newResult = new StudentSubjectResult
            {
                StudentId = studentId,
                SubjectId = subjectId,
                ClassId = classId,
                SchoolYearId = schoolYearId,
                SemesterId = semesterId,
                AverageSemester = semesterAverage,
                CommentResult = commentResult
            };

            await _studentSubjectResultRepository.AddAsync(newResult);
        }
    }

    public async Task CalculateAndSaveStudentSemesterSubjectResultsAsync(
        string studentId, string classId, string schoolYearId, string semesterId)
    {
        // Get all subjects this student has grades for (both numeric and comment-based)
        var subjects = await _context.Grades
            .Where(g => g.StudentId == studentId &&
                       g.ClassId == classId &&
                       g.SchoolYearId == schoolYearId &&
                       g.SemesterId == semesterId)
            .Select(g => g.SubjectId)
            .Distinct()
            .ToListAsync();

        foreach (var subjectId in subjects)
        {
            await CalculateAndSaveSubjectResultAsync(
                studentId, subjectId, classId, schoolYearId, semesterId);
        }
    }

    public async Task CalculateAndSaveSemesterSummaryAsync(
        string studentId, string classId, string schoolYearId, string semesterId)
    {
        // Get all subject results for this semester
        var subjectResults = await _context.StudentSubjectResults
            .Where(ssr => ssr.StudentId == studentId &&
                         ssr.ClassId == classId &&
                         ssr.SchoolYearId == schoolYearId &&
                         ssr.SemesterId == semesterId)
            .ToListAsync();

        if (!subjectResults.Any())
        {
            // No subject results to calculate from
            return;
        }

        // Calculate average of only numeric subjects (skip comment-based)
        var numericSubjects = subjectResults.Where(sr => sr.AverageSemester.HasValue).ToList();

        if (!numericSubjects.Any())
        {
            // No numeric subjects to calculate average from
            return;
        }

        decimal semesterAverage = numericSubjects.Average(sr => sr.AverageSemester!.Value);

        // Get or create semester result
        var semesterResult = await _context.StudentSemesterResults
            .FirstOrDefaultAsync(ssr => ssr.StudentId == studentId &&
                                       ssr.ClassId == classId &&
                                       ssr.SchoolYearId == schoolYearId &&
                                       ssr.SemesterId == semesterId);

        if (semesterResult != null)
        {
            semesterResult.AverageSemester = semesterAverage;
            await _context.SaveChangesAsync();
        }
        else
        {
            semesterResult = new StudentSemesterResult
            {
                StudentId = studentId,
                ClassId = classId,
                SchoolYearId = schoolYearId,
                SemesterId = semesterId,
                AverageSemester = semesterAverage
            };
            _context.StudentSemesterResults.Add(semesterResult);
            await _context.SaveChangesAsync();
        }
    }

    public async Task CalculateAndSaveYearSummaryAsync(
        string studentId, string classId, string schoolYearId)
    {
        // Get semesters with coefficients
        var semester1 = await _context.Semesters.FirstOrDefaultAsync(s => s.SemesterId == "HK1");
        var semester2 = await _context.Semesters.FirstOrDefaultAsync(s => s.SemesterId == "HK2");

        if (semester1 == null || semester2 == null)
            return;

        // Get semester summaries
        var hk1Result = await _context.StudentSemesterResults
            .FirstOrDefaultAsync(ssr => ssr.StudentId == studentId &&
                                       ssr.ClassId == classId &&
                                       ssr.SchoolYearId == schoolYearId &&
                                       ssr.SemesterId == "HK1");

        var hk2Result = await _context.StudentSemesterResults
            .FirstOrDefaultAsync(ssr => ssr.StudentId == studentId &&
                                       ssr.ClassId == classId &&
                                       ssr.SchoolYearId == schoolYearId &&
                                       ssr.SemesterId == "HK2");

        if (hk1Result == null || hk2Result == null)
        {
            // Need both semesters to calculate year average
            return;
        }

        // Calculate weighted year average
        var totalCoef = semester1.Coefficient + semester2.Coefficient;
        var yearAverage = (hk1Result.AverageSemester * semester1.Coefficient +
                          hk2Result.AverageSemester * semester2.Coefficient) / totalCoef;

        // Get or create year result
        var yearResult = await _context.StudentYearResults
            .FirstOrDefaultAsync(syr => syr.StudentId == studentId &&
                                       syr.ClassId == classId &&
                                       syr.SchoolYearId == schoolYearId);

        if (yearResult != null)
        {
            yearResult.AverageSemester1 = hk1Result.AverageSemester;
            yearResult.AverageSemester2 = hk2Result.AverageSemester;
            yearResult.AverageYear = yearAverage;
            await _context.SaveChangesAsync();
        }
        else
        {
            yearResult = new StudentYearResult
            {
                StudentId = studentId,
                ClassId = classId,
                SchoolYearId = schoolYearId,
                AverageSemester1 = hk1Result.AverageSemester,
                AverageSemester2 = hk2Result.AverageSemester,
                AverageYear = yearAverage,
                AcademicPerformanceId = "",
                ConductId = "",
                ResultId = ""
            };
            _context.StudentYearResults.Add(yearResult);
            await _context.SaveChangesAsync();
        }
    }

    public async Task CalculateAndSaveCompleteSemesterAsync(
        string studentId, string classId, string schoolYearId, string semesterId)
    {
        // 1. Calculate subject results
        await CalculateAndSaveStudentSemesterSubjectResultsAsync(
            studentId, classId, schoolYearId, semesterId);

        // 2. Calculate semester summary
        await CalculateAndSaveSemesterSummaryAsync(
            studentId, classId, schoolYearId, semesterId);
    }

    public async Task CalculateAndSaveCompleteYearAsync(
        string studentId, string classId, string schoolYearId)
    {
        // 1. Calculate both semesters
        await CalculateAndSaveCompleteSemesterAsync(
            studentId, classId, schoolYearId, "HK1");
        await CalculateAndSaveCompleteSemesterAsync(
            studentId, classId, schoolYearId, "HK2");

        // 2. Calculate year summary
        await CalculateAndSaveYearSummaryAsync(
            studentId, classId, schoolYearId);
    }

    public async Task CalculateAndSaveClassSemesterAsync(
        string classId, string schoolYearId, string semesterId)
    {
        // Get all students in the class
        var students = await _context.ClassAssignments
            .Where(ca => ca.ClassId == classId)
            .Select(ca => ca.StudentId)
            .Distinct()
            .ToListAsync();

        foreach (var studentId in students)
        {
            await CalculateAndSaveCompleteSemesterAsync(
                studentId, classId, schoolYearId, semesterId);
        }
    }

    public async Task CalculateAndSaveClassYearAsync(
        string classId, string schoolYearId)
    {
        // Get all students in the class
        var students = await _context.ClassAssignments
            .Where(ca => ca.ClassId == classId)
            .Select(ca => ca.StudentId)
            .Distinct()
            .ToListAsync();

        foreach (var studentId in students)
        {
            await CalculateAndSaveCompleteYearAsync(
                studentId, classId, schoolYearId);
        }
    }
}