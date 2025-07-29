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
                .OrderBy(b => b.Weight)
                .ToListAsync();

        public async Task<BagWeightInfo> GetBagWeightInfoAsync(int id, bool trackChanges) =>
            await FindByCondition(b => b.Id.Equals(id), trackChanges)
                .SingleOrDefaultAsync();

        public async Task<BagWeightInfo> GetBagWeightInfoByWeightAsync(double weight, bool trackChanges) =>
            await FindByCondition(b => b.Weight == weight, trackChanges)
                .SingleOrDefaultAsync();

        public void CreateBagWeightInfo(BagWeightInfo bagWeightInfo) => Create(bagWeightInfo);

        public void UpdateBagWeightInfo(BagWeightInfo bagWeightInfo) => Update(bagWeightInfo);

        public void DeleteBagWeightInfo(BagWeightInfo bagWeightInfo) => Delete(bagWeightInfo);
    }
}