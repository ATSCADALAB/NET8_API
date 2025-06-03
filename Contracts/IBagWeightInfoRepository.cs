using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IBagWeightInfoRepository
    {
        Task<IEnumerable<BagWeightInfo>> GetAllBagWeightInfosAsync(bool trackChanges);
        Task<BagWeightInfo> GetBagWeightInfoByWeightAsync(double weight, bool trackChanges);

        void CreateBagWeightInfo(BagWeightInfo BagWeightInfo);

        void UpdateBagWeightInfo(BagWeightInfo BagWeightInfo);

        void DeleteBagWeightInfo(BagWeightInfo BagWeightInfo);
    }
}