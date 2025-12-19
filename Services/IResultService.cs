using API.DTOs;

namespace API.Services;

public interface IResultService
{
    Task<IEnumerable<ResultDto>> GetAllResultsAsync();
    Task<ResultDto?> GetResultByIdAsync(string id);
    Task<ResultDto> CreateResultAsync(CreateResultDto createDto);
    Task<ResultDto> UpdateResultAsync(string id, UpdateResultDto updateDto);
    Task DeleteResultAsync(string id);
}
