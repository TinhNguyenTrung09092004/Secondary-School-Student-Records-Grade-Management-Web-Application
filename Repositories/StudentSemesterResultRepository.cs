using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class StudentSemesterResultRepository : IStudentSemesterResultRepository
{
    private readonly ApplicationDbContext _context;

    public StudentSemesterResultRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<StudentSemesterResult>> GetAllAsync()
    {
        return await _context.StudentSemesterResults.ToListAsync();
    }

    public async Task<StudentSemesterResult?> GetByIdAsync(string studentId, string classId, string schoolYearId, string semesterId)
    {
        return await _context.StudentSemesterResults
            .FirstOrDefaultAsync(ssr => ssr.StudentId == studentId &&
                                       ssr.ClassId == classId &&
                                       ssr.SchoolYearId == schoolYearId &&
                                       ssr.SemesterId == semesterId);
    }

    public async Task<StudentSemesterResult> AddAsync(StudentSemesterResult entity)
    {
        _context.StudentSemesterResults.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(StudentSemesterResult entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string studentId, string classId, string schoolYearId, string semesterId)
    {
        var entity = await GetByIdAsync(studentId, classId, schoolYearId, semesterId);
        if (entity != null)
        {
            _context.StudentSemesterResults.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<StudentSemesterResult>> GetByStudentAsync(string studentId)
    {
        return await _context.StudentSemesterResults
            .Where(ssr => ssr.StudentId == studentId)
            .ToListAsync();
    }
}