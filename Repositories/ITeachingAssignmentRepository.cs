using API.Models;

namespace API.Repositories;

public interface ITeachingAssignmentRepository : IRepositoryInt<TeachingAssignment>
{
    Task<IEnumerable<TeachingAssignment>> GetAllWithDetailsAsync();
    Task<TeachingAssignment?> GetByCompositeKeyAsync(string schoolYearId, string classId, string subjectId);
    Task<IEnumerable<TeachingAssignment>> GetAssignmentsByTeacherIdAsync(string teacherId);
}
