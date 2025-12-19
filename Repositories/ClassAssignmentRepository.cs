using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class ClassAssignmentRepository : IClassAssignmentRepository
{
    private readonly ApplicationDbContext _context;

    public ClassAssignmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ClassAssignment>> GetAllAsync()
    {
        return await _context.ClassAssignments.ToListAsync();
    }

    public async Task<ClassAssignment?> GetByIdAsync(string schoolYearId, string gradeLevelId, string classId, string studentId)
    {
        return await _context.ClassAssignments
            .FirstOrDefaultAsync(ca => ca.SchoolYearId == schoolYearId &&
                                      ca.GradeLevelId == gradeLevelId &&
                                      ca.ClassId == classId &&
                                      ca.StudentId == studentId);
    }

    public async Task<ClassAssignment> AddAsync(ClassAssignment entity)
    {
        _context.ClassAssignments.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(ClassAssignment entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string schoolYearId, string gradeLevelId, string classId, string studentId)
    {
        var entity = await GetByIdAsync(schoolYearId, gradeLevelId, classId, studentId);
        if (entity != null)
        {
            _context.ClassAssignments.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<ClassAssignment>> GetByClassAsync(string classId)
    {
        return await _context.ClassAssignments
            .Where(ca => ca.ClassId == classId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ClassAssignment>> GetByStudentAsync(string studentId)
    {
        return await _context.ClassAssignments
            .Where(ca => ca.StudentId == studentId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ClassAssignment>> GetStudentsByClassIdAsync(string classId)
    {
        return await _context.ClassAssignments
            .Include(ca => ca.Student)
            .Where(ca => ca.ClassId == classId)
            .OrderBy(ca => ca.Student.FullName)
            .ToListAsync();
    }
}
