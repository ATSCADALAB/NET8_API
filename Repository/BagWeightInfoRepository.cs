using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    internal sealed class BagWeightInfoRepository : RepositoryBase<BagWeightInfo>, IBagWeightInfoRepository
    {
        public BagWeightInfoRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<BagWeightInfo>> GetAllBagWeightInfosAsync(bool trackChanges) =>
            await FindAll(trackChanges)
                .ToListAsync();

        public async Task<BagWeightInfo> GetBagWeightInfoByWeightAsync(double weight, bool trackChanges) =>
            await FindByCondition(a => a.Weight == weight, trackChanges)
                .SingleOrDefaultAsync();


        public void CreateBagWeightInfo(BagWeightInfo BagWeightInfo) => Create(BagWeightInfo);

        public void UpdateBagWeightInfo(BagWeightInfo BagWeightInfo) => Update(BagWeightInfo);

        public void DeleteBagWeightInfo(BagWeightInfo BagWeightInfo) => Delete(BagWeightInfo);
    }
}