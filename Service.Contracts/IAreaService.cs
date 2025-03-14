using Shared.DataTransferObjects.Area;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IAreaService
    {
        Task<IEnumerable<AreaDto>> GetAllAreasAsync(bool trackChanges);
        Task<AreaDto> GetAreaAsync(int areaId, bool trackChanges);
        Task<AreaDto> GetAreaByCodeAsync(string areaCode, bool trackChanges);
        Task<AreaDto> CreateAreaAsync(AreaForCreationDto area);
        Task UpdateAreaAsync(int areaId, AreaForUpdateDto areaForUpdate, bool trackChanges);
        Task DeleteAreaAsync(int areaId, bool trackChanges);
    }
}