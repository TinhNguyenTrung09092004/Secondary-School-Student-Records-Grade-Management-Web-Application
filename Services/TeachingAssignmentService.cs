using API.DTOs;
using API.Models;
using API.Repositories;

namespace API.Services;

public class TeachingAssignmentService : ITeachingAssignmentService
{
    private readonly ITeachingAssignmentRepository _teachingAssignmentRepository;
    private readonly ITeacherRepository _teacherRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IClassRepository _classRepository;
    private readonly ISchoolYearRepository _schoolYearRepository;
    private readonly ISubjectRepository _subjectRepository;

    public TeachingAssignmentService(
        ITeachingAssignmentRepository teachingAssignmentRepository,
        ITeacherRepository teacherRepository,
        IDepartmentRepository departmentRepository,
        IClassRepository classRepository,
        ISchoolYearRepository schoolYearRepository,
        ISubjectRepository subjectRepository)
    {
        _teachingAssignmentRepository = teachingAssignmentRepository;
        _teacherRepository = teacherRepository;
        _departmentRepository = departmentRepository;
        _classRepository = classRepository;
        _schoolYearRepository = schoolYearRepository;
        _subjectRepository = subjectRepository;
    }

    public async Task<bool> IsDepartmentHeadAsync(string userId)
    {
        var teacher = await _teacherRepository.GetByUserIdAsync(userId);
        if (teacher == null || string.IsNullOrEmpty(teacher.DepartmentId))
            return false;

        var department = await _departmentRepository.GetByIdAsync(teacher.DepartmentId);
        return department?.HeadTeacherId == teacher.TeacherId;
    }

    public async Task<IEnumerable<DepartmentTeacherDto>> GetDepartmentTeachersAsync(string userId)
    {
        var teacher = await _teacherRepository.GetByUserIdAsync(userId);
        if (teacher == null || string.IsNullOrEmpty(teacher.DepartmentId))
            return Enumerable.Empty<DepartmentTeacherDto>();

        var department = await _departmentRepository.GetByIdAsync(teacher.DepartmentId);
        if (department?.HeadTeacherId != teacher.TeacherId)
            return Enumerable.Empty<DepartmentTeacherDto>();

        var teachers = await _teacherRepository.GetTeachersByDepartmentIdAsync(teacher.DepartmentId);

        return teachers.Select(t => new DepartmentTeacherDto
        {
            TeacherId = t.TeacherId,
            TeacherName = t.TeacherName,
            SubjectId = t.SubjectId,
            SubjectName = t.Subject.SubjectName,
            IsHeadTeacher = t.TeacherId == department.HeadTeacherId
        });
    }

    public async Task<IEnumerable<TeachingAssignmentDto>> GetAllTeachingAssignmentsAsync()
    {
        var assignments = await _teachingAssignmentRepository.GetAllWithDetailsAsync();

        return assignments.Select(ta => new TeachingAssignmentDto
        {
            RowNumber = ta.RowNumber,
            SchoolYearId = ta.SchoolYearId,
            SchoolYearName = ta.SchoolYear.SchoolYearName,
            ClassId = ta.ClassId,
            ClassName = ta.Class.ClassName,
            SubjectId = ta.SubjectId,
            SubjectName = ta.Subject.SubjectName,
            TeacherId = ta.TeacherId,
            TeacherName = ta.Teacher.TeacherName
        });
    }

    public async Task<TeachingAssignmentDto?> CreateTeachingAssignmentAsync(string userId, CreateTeachingAssignmentDto createDto)
    {
        // Verify the user is a department head
        var currentTeacher = await _teacherRepository.GetByUserIdAsync(userId);
        if (currentTeacher == null || string.IsNullOrEmpty(currentTeacher.DepartmentId))
            return null;

        var department = await _departmentRepository.GetByIdAsync(currentTeacher.DepartmentId);
        if (department?.HeadTeacherId != currentTeacher.TeacherId)
            return null;

        // Verify the teacher to be assigned belongs to the same department
        var teacherToAssign = await _teacherRepository.GetByIdAsync(createDto.TeacherId);
        if (teacherToAssign == null || teacherToAssign.DepartmentId != currentTeacher.DepartmentId)
            return null;

        // Verify school year, class, and subject exist
        var schoolYear = await _schoolYearRepository.GetByIdAsync(createDto.SchoolYearId);
        if (schoolYear == null)
            return null;

        var classEntity = await _classRepository.GetByIdAsync(createDto.ClassId);
        if (classEntity == null)
            return null;

        var subject = await _subjectRepository.GetByIdAsync(createDto.SubjectId);
        if (subject == null)
            return null;

        // Check if assignment already exists
        var existingAssignment = await _teachingAssignmentRepository.GetByCompositeKeyAsync(
            createDto.SchoolYearId, createDto.ClassId, createDto.SubjectId);

        if (existingAssignment != null)
            return null;

        // Create new assignment
        var assignment = new TeachingAssignment
        {
            SchoolYearId = createDto.SchoolYearId,
            ClassId = createDto.ClassId,
            SubjectId = createDto.SubjectId,
            TeacherId = createDto.TeacherId
        };

        await _teachingAssignmentRepository.AddAsync(assignment);

        return new TeachingAssignmentDto
        {
            RowNumber = assignment.RowNumber,
            SchoolYearId = assignment.SchoolYearId,
            SchoolYearName = schoolYear.SchoolYearName,
            ClassId = assignment.ClassId,
            ClassName = classEntity.ClassName,
            SubjectId = assignment.SubjectId,
            SubjectName = subject.SubjectName,
            TeacherId = assignment.TeacherId,
            TeacherName = teacherToAssign.TeacherName
        };
    }

    public async Task<TeachingAssignmentDto?> UpdateTeachingAssignmentAsync(int id, string userId, UpdateTeachingAssignmentDto updateDto)
    {
        // Verify the user is a department head
        var currentTeacher = await _teacherRepository.GetByUserIdAsync(userId);
        if (currentTeacher == null || string.IsNullOrEmpty(currentTeacher.DepartmentId))
            return null;

        var department = await _departmentRepository.GetByIdAsync(currentTeacher.DepartmentId);
        if (department?.HeadTeacherId != currentTeacher.TeacherId)
            return null;

        // Get the old assignment
        var oldAssignment = await _teachingAssignmentRepository.GetByIdAsync(id);
        if (oldAssignment == null)
            return null;

        // Verify the current teacher in the assignment belongs to the same department
        var currentAssignedTeacher = await _teacherRepository.GetByIdAsync(oldAssignment.TeacherId);
        if (currentAssignedTeacher == null || currentAssignedTeacher.DepartmentId != currentTeacher.DepartmentId)
            return null;

        // Verify the new teacher to be assigned belongs to the same department
        var newTeacher = await _teacherRepository.GetByIdAsync(updateDto.TeacherId);
        if (newTeacher == null || newTeacher.DepartmentId != currentTeacher.DepartmentId)
            return null;

        // Verify school year, class, and subject exist
        var schoolYear = await _schoolYearRepository.GetByIdAsync(updateDto.SchoolYearId);
        if (schoolYear == null)
            return null;

        var classEntity = await _classRepository.GetByIdAsync(updateDto.ClassId);
        if (classEntity == null)
            return null;

        var subject = await _subjectRepository.GetByIdAsync(updateDto.SubjectId);
        if (subject == null)
            return null;

        // Check if the composite key has changed
        bool compositeKeyChanged = oldAssignment.SchoolYearId != updateDto.SchoolYearId ||
                                   oldAssignment.ClassId != updateDto.ClassId ||
                                   oldAssignment.SubjectId != updateDto.SubjectId;

        if (compositeKeyChanged)
        {
            // Check if new composite key already exists
            var existingAssignment = await _teachingAssignmentRepository.GetByCompositeKeyAsync(
                updateDto.SchoolYearId, updateDto.ClassId, updateDto.SubjectId);

            if (existingAssignment != null && existingAssignment.RowNumber != id)
                return null; // Conflict: assignment already exists

            // Delete old assignment and create new one
            await _teachingAssignmentRepository.DeleteAsync(id);

            var newAssignment = new TeachingAssignment
            {
                SchoolYearId = updateDto.SchoolYearId,
                ClassId = updateDto.ClassId,
                SubjectId = updateDto.SubjectId,
                TeacherId = updateDto.TeacherId
            };

            await _teachingAssignmentRepository.AddAsync(newAssignment);

            return new TeachingAssignmentDto
            {
                RowNumber = newAssignment.RowNumber,
                SchoolYearId = newAssignment.SchoolYearId,
                SchoolYearName = schoolYear.SchoolYearName,
                ClassId = newAssignment.ClassId,
                ClassName = classEntity.ClassName,
                SubjectId = newAssignment.SubjectId,
                SubjectName = subject.SubjectName,
                TeacherId = newAssignment.TeacherId,
                TeacherName = newTeacher.TeacherName
            };
        }
        else
        {
            // Only teacher changed, just update
            oldAssignment.TeacherId = updateDto.TeacherId;
            await _teachingAssignmentRepository.UpdateAsync(oldAssignment);

            return new TeachingAssignmentDto
            {
                RowNumber = oldAssignment.RowNumber,
                SchoolYearId = oldAssignment.SchoolYearId,
                SchoolYearName = schoolYear.SchoolYearName,
                ClassId = oldAssignment.ClassId,
                ClassName = classEntity.ClassName,
                SubjectId = oldAssignment.SubjectId,
                SubjectName = subject.SubjectName,
                TeacherId = oldAssignment.TeacherId,
                TeacherName = newTeacher.TeacherName
            };
        }
    }

    public async Task<bool> DeleteTeachingAssignmentAsync(int id, string userId)
    {
        // Verify the user is a department head
        var currentTeacher = await _teacherRepository.GetByUserIdAsync(userId);
        if (currentTeacher == null || string.IsNullOrEmpty(currentTeacher.DepartmentId))
            return false;

        var department = await _departmentRepository.GetByIdAsync(currentTeacher.DepartmentId);
        if (department?.HeadTeacherId != currentTeacher.TeacherId)
            return false;

        // Get the assignment
        var assignment = await _teachingAssignmentRepository.GetByIdAsync(id);
        if (assignment == null)
            return false;

        // Verify the teacher in the assignment belongs to the same department
        var teacherInAssignment = await _teacherRepository.GetByIdAsync(assignment.TeacherId);
        if (teacherInAssignment == null || teacherInAssignment.DepartmentId != currentTeacher.DepartmentId)
            return false;

        await _teachingAssignmentRepository.DeleteAsync(id);
        return true;
    }
}
