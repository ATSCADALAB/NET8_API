using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    internal sealed class DistributorRepository : RepositoryBase<Distributor>, IDistributorRepository
    {
        public DistributorRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<Distributor>> GetAllDistributorsAsync(bool trackChanges) =>
            await FindAll(trackChanges)
            .Include(d => d.Area)
                .OrderBy(d => d.DistributorCode)

                .ToListAsync();

        public async Task<Distributor> GetDistributorByIdAsync(int distributorId, bool trackChanges) =>
            await FindByCondition(d => d.Id == distributorId, trackChanges)
                .Include(d => d.Area)
                .SingleOrDefaultAsync();

        public async Task<Distributor> GetDistributorByCodeAsync(string distributorCode, bool trackChanges) =>
            await FindByCondition(d => d.DistributorCode == distributorCode, trackChanges)
                .Include(d => d.Area)
                .SingleOrDefaultAsync();

        public async Task<IEnumerable<Distributor>> GetDistributorsByAreaAsync(int areaId, bool trackChanges) =>
            await FindByCondition(d => d.AreaId == areaId, trackChanges)
                .Include(d => d.Area)
                .OrderBy(d => d.DistributorCode)
                .ToListAsync();

        public void CreateDistributor(Distributor distributor) => Create(distributor);

        public void UpdateDistributor(Distributor distributor) => Update(distributor);

        public void DeleteDistributor(Distributor distributor) => Delete(distributor);
    }
}