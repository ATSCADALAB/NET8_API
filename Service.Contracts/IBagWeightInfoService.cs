using Entities.Models;
using Shared.DataTransferObjects.BagWeightInfo;

namespace Service.Contracts
{
    public interface IBagWeightInfoService
    {
        Task<IEnumerable<BagWeightInfoDto>> GetAllBagWeightInfosAsync(bool trackChanges);
        Task<BagWeightInfoDto> GetBagWeightInfoAsync(int bagWeightInfoId, bool trackChanges);
        Task<BagWeightInfoDto> GetBagWeightInfoByWeightAsync(double weight, bool trackChanges, int lineID);
        Task<BagWeightInfoDto> CreateBagWeightInfoAsync(BagWeightInfoForCreationDto bagWeightInfo);
        Task UpdateBagWeightInfoAsync(int bagWeightInfoId, BagWeightInfoForUpdateDto bagWeightInfoForUpdate, bool trackChanges,int lineID);
        Task DeleteBagWeightInfoAsync(int bagWeightInfoId, bool trackChanges);
    }
}