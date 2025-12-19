using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class TeacherRepository : RepositoryBase<Teacher, string>, ITeacherRepository
{
    public TeacherRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Teacher?> GetByUserIdAsync(string userId)
    {
        return await _dbSet
            .Include(t => t.Subject)
            .Include(t => t.Department)
            .FirstOrDefaultAsync(t => t.UserId == userId);
    }

    public async Task<IEnumerable<Teacher>> GetTeachersByDepartmentIdAsync(string departmentId)
    {
        return await _dbSet
            .Include(t => t.Subject)
            .Where(t => t.DepartmentId == departmentId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Teacher>> GetAllWithSubjectAsync()
    {
        return await _dbSet
            .Include(t => t.Subject)
            .OrderBy(t => t.TeacherName)
            .ToListAsync();
    }
}
