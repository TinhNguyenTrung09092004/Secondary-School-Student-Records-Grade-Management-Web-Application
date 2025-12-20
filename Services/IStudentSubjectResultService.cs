namespace API.Services;

public interface IStudentSubjectResultService
{
    /// <summary>
    /// Calculate and save subject result for a specific student, subject, and semester
    /// </summary>
    Task CalculateAndSaveSubjectResultAsync(
        string studentId, string subjectId, string classId,
        string schoolYearId, string semesterId);

    /// <summary>
    /// Calculate and save all subject results for a student in a semester
    /// </summary>
    Task CalculateAndSaveStudentSemesterSubjectResultsAsync(
        string studentId, string classId, string schoolYearId, string semesterId);

    /// <summary>
    /// Calculate and save semester summary (average of all subjects) for a student
    /// Populates STUDENT_SEMESTER_RESULT.AverageSemester
    /// </summary>
    Task CalculateAndSaveSemesterSummaryAsync(
        string studentId, string classId, string schoolYearId, string semesterId);

    /// <summary>
    /// Calculate and save year summary (semester averages and year average) for a student
    /// Populates STUDENT_YEAR_RESULT.AverageSemester1, AverageSemester2, AverageYear
    /// </summary>
    Task CalculateAndSaveYearSummaryAsync(
        string studentId, string classId, string schoolYearId);

    /// <summary>
    /// Calculate everything for a student in a semester: subject results + semester summary
    /// </summary>
    Task CalculateAndSaveCompleteSemesterAsync(
        string studentId, string classId, string schoolYearId, string semesterId);

    /// <summary>
    /// Calculate everything for a student for the year: both semesters + year summary
    /// </summary>
    Task CalculateAndSaveCompleteYearAsync(
        string studentId, string classId, string schoolYearId);

    /// <summary>
    /// Calculate everything for all students in a class for a semester
    /// </summary>
    Task CalculateAndSaveClassSemesterAsync(
        string classId, string schoolYearId, string semesterId);

    /// <summary>
    /// Calculate everything for all students in a class for the year
    /// </summary>
    Task CalculateAndSaveClassYearAsync(
        string classId, string schoolYearId);
}