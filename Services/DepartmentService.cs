using API.DTOs;
using API.Models;
using API.Repositories;

namespace API.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ITeacherRepository _teacherRepository;

    public DepartmentService(IDepartmentRepository departmentRepository, ITeacherRepository teacherRepository)
    {
        _departmentRepository = departmentRepository;
        _teacherRepository = teacherRepository;
    }

    public async Task<IEnumerable<DepartmentResponseDto>> GetAllDepartmentsAsync()
    {
        var departments = await _departmentRepository.GetAllWithDetailsAsync();
        return departments.Select(d => new DepartmentResponseDto
        {
            DepartmentId = d.DepartmentId,
            DepartmentName = d.DepartmentName,
            HeadTeacherId = d.HeadTeacherId,
            HeadTeacherName = d.HeadTeacher?.TeacherName,
            TeacherCount = d.Teachers.Count
        });
    }

    public async Task<DepartmentDetailDto?> GetDepartmentByIdAsync(string id)
    {
        var department = await _departmentRepository.GetByIdWithDetailsAsync(id);
        if (department == null)
            return null;

        return new DepartmentDetailDto
        {
            DepartmentId = department.DepartmentId,
            DepartmentName = department.DepartmentName,
            HeadTeacherId = department.HeadTeacherId,
            HeadTeacherName = department.HeadTeacher?.TeacherName,
            Teachers = department.Teachers.Select(t => new TeacherInDepartmentDto
            {
                TeacherId = t.TeacherId,
                TeacherName = t.TeacherName,
                SubjectName = t.Subject.SubjectName
            }).ToList()
        };
    }

    public async Task<DepartmentResponseDto?> CreateDepartmentAsync(CreateDepartmentDto createDto)
    {
        // Check if department ID already exists
        var existingDepartment = await _departmentRepository.GetByIdAsync(createDto.DepartmentId);
        if (existingDepartment != null)
            return null;

        // Validate head teacher if provided
        if (!string.IsNullOrEmpty(createDto.HeadTeacherId))
        {
            var teacher = await _teacherRepository.GetByIdAsync(createDto.HeadTeacherId);
            if (teacher == null)
                return null;
        }

        var department = new Department
        {
            DepartmentId = createDto.DepartmentId,
            DepartmentName = createDto.DepartmentName,
            HeadTeacherId = createDto.HeadTeacherId
        };

        await _departmentRepository.AddAsync(department);

        var createdDepartment = await _departmentRepository.GetByIdWithDetailsAsync(department.DepartmentId);
        return new DepartmentResponseDto
        {
            DepartmentId = createdDepartment!.DepartmentId,
            DepartmentName = createdDepartment.DepartmentName,
            HeadTeacherId = createdDepartment.HeadTeacherId,
            HeadTeacherName = createdDepartment.HeadTeacher?.TeacherName,
            TeacherCount = createdDepartment.Teachers.Count
        };
    }

    public async Task<DepartmentResponseDto?> UpdateDepartmentAsync(string id, UpdateDepartmentDto updateDto)
    {
        var department = await _departmentRepository.GetByIdAsync(id);
        if (department == null)
            return null;

        // Validate head teacher if provided
        if (!string.IsNullOrEmpty(updateDto.HeadTeacherId))
        {
            var teacher = await _teacherRepository.GetByIdAsync(updateDto.HeadTeacherId);
            if (teacher == null)
                return null;
        }

        department.DepartmentName = updateDto.DepartmentName;
        department.HeadTeacherId = updateDto.HeadTeacherId;

        await _departmentRepository.UpdateAsync(department);

        var updatedDepartment = await _departmentRepository.GetByIdWithDetailsAsync(id);
        return new DepartmentResponseDto
        {
            DepartmentId = updatedDepartment!.DepartmentId,
            DepartmentName = updatedDepartment.DepartmentName,
            HeadTeacherId = updatedDepartment.HeadTeacherId,
            HeadTeacherName = updatedDepartment.HeadTeacher?.TeacherName,
            TeacherCount = updatedDepartment.Teachers.Count
        };
    }

    public async Task<bool> DeleteDepartmentAsync(string id)
    {
        var department = await _departmentRepository.GetByIdWithDetailsAsync(id);
        if (department == null)
            return false;

        // Check if department has teachers assigned
        if (department.Teachers.Any())
            return false;

        await _departmentRepository.DeleteAsync(id);
        return true;
    }
}
