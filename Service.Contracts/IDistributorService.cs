using Shared.DataTransferObjects.Distributor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IDistributorService
    {
        Task<IEnumerable<DistributorDto>> GetAllDistributorsAsync(bool trackChanges);
        Task<DistributorDto> GetDistributorAsync(int distributorId, bool trackChanges);
        Task<DistributorDto> GetDistributorByCodeAsync(string distributorCode, bool trackChanges);
        Task<IEnumerable<DistributorDto>> GetDistributorsByAreaAsync(int areaId, bool trackChanges);
        Task<DistributorDto> CreateDistributorAsync(DistributorForCreationDto distributor);
        Task UpdateDistributorAsync(int distributorId, DistributorForUpdateDto distributorForUpdate, bool trackChanges);
        Task DeleteDistributorAsync(int distributorId, bool trackChanges);
        
    }
}