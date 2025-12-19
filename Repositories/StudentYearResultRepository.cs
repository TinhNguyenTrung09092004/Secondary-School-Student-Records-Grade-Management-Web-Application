using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class StudentYearResultRepository : IStudentYearResultRepository
{
    private readonly ApplicationDbContext _context;

    public StudentYearResultRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<StudentYearResult>> GetAllAsync()
    {
        return await _context.StudentYearResults.ToListAsync();
    }

    public async Task<StudentYearResult?> GetByIdAsync(string studentId, string classId, string schoolYearId)
    {
        return await _context.StudentYearResults
            .FirstOrDefaultAsync(syr => syr.StudentId == studentId &&
                                       syr.ClassId == classId &&
                                       syr.SchoolYearId == schoolYearId);
    }

    public async Task<StudentYearResult> AddAsync(StudentYearResult entity)
    {
        _context.StudentYearResults.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(StudentYearResult entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string studentId, string classId, string schoolYearId)
    {
        var entity = await GetByIdAsync(studentId, classId, schoolYearId);
        if (entity != null)
        {
            _context.StudentYearResults.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<StudentYearResult>> GetByStudentAsync(string studentId)
    {
        return await _context.StudentYearResults
            .Where(syr => syr.StudentId == studentId)
            .ToListAsync();
    }
}
