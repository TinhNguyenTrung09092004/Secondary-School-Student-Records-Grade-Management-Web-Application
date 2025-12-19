using API.Models;

namespace API.Repositories;

public interface IGradeRepository : IRepositoryInt<Grade>
{
    Task<IEnumerable<Grade>> GetGradesByClassSubjectSemesterAsync(string classId, string subjectId, string semesterId, string schoolYearId);
    Task<Grade?> GetGradeByDetailsAsync(string studentId, string subjectId, string semesterId, string schoolYearId, string classId, string gradeTypeId);
    Task<IEnumerable<Grade>> GetGradesByStudentAsync(string studentId, string classId, string subjectId, string semesterId, string schoolYearId);
}
