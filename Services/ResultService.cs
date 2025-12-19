using API.DTOs;
using API.Models;
using API.Repositories;

namespace API.Services;

public class ResultService : IResultService
{
    private readonly IResultRepository _repository;

    public ResultService(IResultRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ResultDto>> GetAllResultsAsync()
    {
        var results = await _repository.GetAllAsync();
        return results.Select(r => new ResultDto
        {
            ResultId = r.ResultId,
            ResultName = r.ResultName
        });
    }

    public async Task<ResultDto?> GetResultByIdAsync(string id)
    {
        var result = await _repository.GetByIdAsync(id);
        if (result == null) return null;

        return new ResultDto
        {
            ResultId = result.ResultId,
            ResultName = result.ResultName
        };
    }

    public async Task<ResultDto> CreateResultAsync(CreateResultDto createDto)
    {
        var result = new Result
        {
            ResultId = createDto.ResultId,
            ResultName = createDto.ResultName
        };

        var created = await _repository.AddAsync(result);

        return new ResultDto
        {
            ResultId = created.ResultId,
            ResultName = created.ResultName
        };
    }

    public async Task<ResultDto> UpdateResultAsync(string id, UpdateResultDto updateDto)
    {
        var result = await _repository.GetByIdAsync(id);
        if (result == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy kết quả với mã {id}");
        }

        result.ResultName = updateDto.ResultName;

        await _repository.UpdateAsync(result);

        return new ResultDto
        {
            ResultId = result.ResultId,
            ResultName = result.ResultName
        };
    }

    public async Task DeleteResultAsync(string id)
    {
        var result = await _repository.GetByIdAsync(id);
        if (result == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy kết quả với mã {id}");
        }

        await _repository.DeleteAsync(id);
    }
}
