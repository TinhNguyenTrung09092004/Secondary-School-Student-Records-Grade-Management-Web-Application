using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class StudentRepository : RepositoryBase<Student, string>, IStudentRepository
{
    public StudentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Student>> GetByClassAsync(string classId)
    {
        return await _context.ClassAssignments
            .Where(ca => ca.ClassId == classId)
            .Join(_context.Students,
                ca => ca.StudentId,
                s => s.StudentId,
                (ca, s) => s)
            .ToListAsync();
    }
}
