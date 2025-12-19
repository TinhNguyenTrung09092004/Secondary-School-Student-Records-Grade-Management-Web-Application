using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class DepartmentRepository : RepositoryBase<Department, string>, IDepartmentRepository
{
    public DepartmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Department?> GetByIdWithDetailsAsync(string id)
    {
        return await _dbSet
            .Include(d => d.HeadTeacher)
            .Include(d => d.Teachers)
            .FirstOrDefaultAsync(d => d.DepartmentId == id);
    }

    public async Task<IEnumerable<Department>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(d => d.HeadTeacher)
            .Include(d => d.Teachers)
            .ToListAsync();
    }
}
