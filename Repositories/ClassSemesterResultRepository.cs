using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class ClassSemesterResultRepository : IClassSemesterResultRepository
{
    private readonly ApplicationDbContext _context;

    public ClassSemesterResultRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ClassSemesterResult>> GetAllAsync()
    {
        return await _context.ClassSemesterResults.ToListAsync();
    }

    public async Task<ClassSemesterResult?> GetByIdAsync(string classId, string schoolYearId, string semesterId)
    {
        return await _context.ClassSemesterResults
            .FirstOrDefaultAsync(csr => csr.ClassId == classId &&
                                       csr.SchoolYearId == schoolYearId &&
                                       csr.SemesterId == semesterId);
    }

    public async Task<ClassSemesterResult> AddAsync(ClassSemesterResult entity)
    {
        _context.ClassSemesterResults.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(ClassSemesterResult entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string classId, string schoolYearId, string semesterId)
    {
        var entity = await GetByIdAsync(classId, schoolYearId, semesterId);
        if (entity != null)
        {
            _context.ClassSemesterResults.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<ClassSemesterResult>> GetByClassAsync(string classId)
    {
        return await _context.ClassSemesterResults
            .Where(csr => csr.ClassId == classId)
            .ToListAsync();
    }
}
