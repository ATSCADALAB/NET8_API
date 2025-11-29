using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IBagWeightInfoRepository
    {
        Task<IEnumerable<BagWeightInfo>> GetAllBagWeightInfosAsync(bool trackChanges);
        Task<BagWeightInfo> GetBagWeightInfoAsync(int id, bool trackChanges);
        Task<BagWeightInfo> GetBagWeightInfoByWeightAsync(double weight, bool trackChanges,int line);
        void CreateBagWeightInfo(BagWeightInfo bagWeightInfo);
        void UpdateBagWeightInfo(BagWeightInfo bagWeightInfo);
        void DeleteBagWeightInfo(BagWeightInfo bagWeightInfo);
    }
}