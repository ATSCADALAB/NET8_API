using Shared.DataTransferObjects;
using Shared.DataTransferObjects.Distributor;

namespace Service.Contracts
{
    public interface IDistributorService
    {
        Task<IEnumerable<DistributorDto>> GetAllDistributorsAsync(bool trackChanges);
        Task<IEnumerable<DistributorDto>> GetDistributorsByNameAsync(string Name);
        Task<DistributorDto> GetDistributorAsync(long distributorId, bool trackChanges);
        Task<DistributorDto> CreateDistributorAsync(DistributorForCreationDto distributor);
        Task DeleteDistributorAsync(long distributorId, bool trackChanges);
        Task UpdateDistributorAsync(long distributorId, DistributorForUpdateDto distributorForUpdate, bool trackChanges);
        Task CreateDistributorsBatchAsync(IEnumerable<DistributorForCreationDto> distributors);
    }
}