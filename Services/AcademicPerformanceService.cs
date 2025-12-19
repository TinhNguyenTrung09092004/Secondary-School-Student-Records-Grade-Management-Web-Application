using API.DTOs;
using API.Models;
using API.Repositories;

namespace API.Services;

public class AcademicPerformanceService : IAcademicPerformanceService
{
    private readonly IAcademicPerformanceRepository _repository;

    public AcademicPerformanceService(IAcademicPerformanceRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<AcademicPerformanceDto>> GetAllAcademicPerformancesAsync()
    {
        var academicPerformances = await _repository.GetAllAsync();
        return academicPerformances.Select(ap => new AcademicPerformanceDto
        {
            AcademicPerformanceId = ap.AcademicPerformanceId,
            AcademicPerformanceName = ap.AcademicPerformanceName
        });
    }

    public async Task<AcademicPerformanceDto?> GetAcademicPerformanceByIdAsync(string id)
    {
        var academicPerformance = await _repository.GetByIdAsync(id);
        if (academicPerformance == null) return null;

        return new AcademicPerformanceDto
        {
            AcademicPerformanceId = academicPerformance.AcademicPerformanceId,
            AcademicPerformanceName = academicPerformance.AcademicPerformanceName
        };
    }

    public async Task<AcademicPerformanceDto> CreateAcademicPerformanceAsync(CreateAcademicPerformanceDto createDto)
    {
        var academicPerformance = new AcademicPerformance
        {
            AcademicPerformanceId = createDto.AcademicPerformanceId,
            AcademicPerformanceName = createDto.AcademicPerformanceName
        };

        var created = await _repository.AddAsync(academicPerformance);

        return new AcademicPerformanceDto
        {
            AcademicPerformanceId = created.AcademicPerformanceId,
            AcademicPerformanceName = created.AcademicPerformanceName
        };
    }

    public async Task<AcademicPerformanceDto> UpdateAcademicPerformanceAsync(string id, UpdateAcademicPerformanceDto updateDto)
    {
        var academicPerformance = await _repository.GetByIdAsync(id);
        if (academicPerformance == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy học lực với mã {id}");
        }

        academicPerformance.AcademicPerformanceName = updateDto.AcademicPerformanceName;

        await _repository.UpdateAsync(academicPerformance);

        return new AcademicPerformanceDto
        {
            AcademicPerformanceId = academicPerformance.AcademicPerformanceId,
            AcademicPerformanceName = academicPerformance.AcademicPerformanceName
        };
    }

    public async Task DeleteAcademicPerformanceAsync(string id)
    {
        var academicPerformance = await _repository.GetByIdAsync(id);
        if (academicPerformance == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy học lực với mã {id}");
        }

        await _repository.DeleteAsync(id);
    }
}
