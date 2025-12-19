using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class ClassSubjectResultRepository : IClassSubjectResultRepository
{
    private readonly ApplicationDbContext _context;

    public ClassSubjectResultRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ClassSubjectResult>> GetAllAsync()
    {
        return await _context.ClassSubjectResults.ToListAsync();
    }

    public async Task<ClassSubjectResult?> GetByIdAsync(string classId, string schoolYearId, string subjectId, string semesterId)
    {
        return await _context.ClassSubjectResults
            .FirstOrDefaultAsync(csr => csr.ClassId == classId &&
                                       csr.SchoolYearId == schoolYearId &&
                                       csr.SubjectId == subjectId &&
                                       csr.SemesterId == semesterId);
    }

    public async Task<ClassSubjectResult> AddAsync(ClassSubjectResult entity)
    {
        _context.ClassSubjectResults.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(ClassSubjectResult entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string classId, string schoolYearId, string subjectId, string semesterId)
    {
        var entity = await GetByIdAsync(classId, schoolYearId, subjectId, semesterId);
        if (entity != null)
        {
            _context.ClassSubjectResults.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<ClassSubjectResult>> GetByClassAsync(string classId)
    {
        return await _context.ClassSubjectResults
            .Where(csr => csr.ClassId == classId)
            .ToListAsync();
    }
}
