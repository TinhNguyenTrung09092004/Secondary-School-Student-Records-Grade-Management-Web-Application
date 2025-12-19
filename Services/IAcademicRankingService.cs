using API.Models;

namespace API.Services;

public interface IAcademicRankingService
{
    /// <summary>
    /// Calculate academic performance ranking (Tốt, Khá, Đạt, Chưa đạt) for a semester
    /// </summary>
    Task<string> CalculateSemesterAcademicPerformanceAsync(
        string studentId, string classId, string schoolYearId, string semesterId);

    /// <summary>
    /// Calculate academic performance ranking (Tốt, Khá, Đạt, Chưa đạt) for the year
    /// </summary>
    Task<string> CalculateYearAcademicPerformanceAsync(
        string studentId, string classId, string schoolYearId);

    /// <summary>
    /// Calculate year result title (Xuất sắc, Giỏi, Tiên tiến, or empty)
    /// </summary>
    Task<string> CalculateYearResultTitleAsync(
        string studentId, string classId, string schoolYearId);

    /// <summary>
    /// Update semester academic performance for a student
    /// </summary>
    Task UpdateSemesterAcademicPerformanceAsync(
        string studentId, string classId, string schoolYearId, string semesterId);

    /// <summary>
    /// Update year academic performance and result title for a student
    /// </summary>
    Task UpdateYearAcademicPerformanceAsync(
        string studentId, string classId, string schoolYearId);
}