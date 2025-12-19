using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class GradeRepository : RepositoryBase<Grade, int>, IGradeRepository
{
    public GradeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Grade>> GetGradesByClassSubjectSemesterAsync(string classId, string subjectId, string semesterId, string schoolYearId)
    {
        return await _dbSet
            .Include(g => g.Student)
            .Include(g => g.GradeType)
            .Where(g => g.ClassId == classId &&
                       g.SubjectId == subjectId &&
                       g.SemesterId == semesterId &&
                       g.SchoolYearId == schoolYearId)
            .OrderBy(g => g.Student.FullName)
            .ThenBy(g => g.GradeType.GradeTypeName)
            .ToListAsync();
    }

    public async Task<Grade?> GetGradeByDetailsAsync(string studentId, string subjectId, string semesterId, string schoolYearId, string classId, string gradeTypeId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(g => g.StudentId == studentId &&
                                    g.SubjectId == subjectId &&
                                    g.SemesterId == semesterId &&
                                    g.SchoolYearId == schoolYearId &&
                                    g.ClassId == classId &&
                                    g.GradeTypeId == gradeTypeId);
    }

    public async Task<IEnumerable<Grade>> GetGradesByStudentAsync(string studentId, string classId, string subjectId, string semesterId, string schoolYearId)
    {
        return await _dbSet
            .Include(g => g.GradeType)
            .Where(g => g.StudentId == studentId &&
                       g.ClassId == classId &&
                       g.SubjectId == subjectId &&
                       g.SemesterId == semesterId &&
                       g.SchoolYearId == schoolYearId)
            .ToListAsync();
    }
}
