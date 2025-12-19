using API.DTOs;

namespace API.Services;

public interface IEthnicityService
{
    Task<List<EthnicityDto>> GetAllEthnicityAsync();
    Task<EthnicityDto?> GetEthnicityByIdAsync(string maDanToc);
    Task<EthnicityDto?> CreateEthnicityAsync(CreateEthnicityDto createDto);
    Task<EthnicityDto?> UpdateEthnicityAsync(string maDanToc, UpdateEthnicityDto updateDto);
    Task<bool> DeleteEthnicityAsync(string maDanToc);
}
