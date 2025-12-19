using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class StudentSubjectResultRepository : IStudentSubjectResultRepository
{
    private readonly ApplicationDbContext _context;

    public StudentSubjectResultRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<StudentSubjectResult>> GetAllAsync()
    {
        return await _context.StudentSubjectResults.ToListAsync();
    }

    public async Task<StudentSubjectResult?> GetByIdAsync(string studentId, string classId, string schoolYearId, string subjectId, string semesterId)
    {
        return await _context.StudentSubjectResults
            .FirstOrDefaultAsync(ssr => ssr.StudentId == studentId &&
                                       ssr.ClassId == classId &&
                                       ssr.SchoolYearId == schoolYearId &&
                                       ssr.SubjectId == subjectId &&
                                       ssr.SemesterId == semesterId);
    }

    public async Task<StudentSubjectResult> AddAsync(StudentSubjectResult entity)
    {
        _context.StudentSubjectResults.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(StudentSubjectResult entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string studentId, string classId, string schoolYearId, string subjectId, string semesterId)
    {
        var entity = await GetByIdAsync(studentId, classId, schoolYearId, subjectId, semesterId);
        if (entity != null)
        {
            _context.StudentSubjectResults.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<StudentSubjectResult>> GetByStudentAsync(string studentId)
    {
        return await _context.StudentSubjectResults
            .Where(ssr => ssr.StudentId == studentId)
            .ToListAsync();
    }
}
