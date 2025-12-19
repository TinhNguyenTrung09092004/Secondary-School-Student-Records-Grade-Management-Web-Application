using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class TeachingAssignmentRepository : RepositoryBase<TeachingAssignment, int>, ITeachingAssignmentRepository
{
    public TeachingAssignmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TeachingAssignment>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(ta => ta.SchoolYear)
            .Include(ta => ta.Class)
            .Include(ta => ta.Subject)
            .Include(ta => ta.Teacher)
            .ToListAsync();
    }

    public async Task<TeachingAssignment?> GetByCompositeKeyAsync(string schoolYearId, string classId, string subjectId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(ta =>
                ta.SchoolYearId == schoolYearId &&
                ta.ClassId == classId &&
                ta.SubjectId == subjectId);
    }

    public async Task<IEnumerable<TeachingAssignment>> GetAssignmentsByTeacherIdAsync(string teacherId)
    {
        return await _dbSet
            .Include(ta => ta.SchoolYear)
            .Include(ta => ta.Class)
            .Include(ta => ta.Subject)
            .Where(ta => ta.TeacherId == teacherId)
            .OrderBy(ta => ta.Class.ClassName)
            .ThenBy(ta => ta.Subject.SubjectName)
            .ToListAsync();
    }
}
