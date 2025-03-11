using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace Repository
{
    internal sealed class DistributorRepository : RepositoryBase<Distributor>, IDistributorRepository
    {
        public DistributorRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public void CreateDistributor(Distributor distributor)
        {
            Create(distributor);
        }

        public void DeleteDistributor(Distributor distributor)
        {
            Delete(distributor);
        }
        // Triển khai AddRangeAsync
        public async Task AddRangeAsync(IEnumerable<Distributor> distributors)
        {
            await RepositoryContext.Set<Distributor>().AddRangeAsync(distributors);
        }
        public async Task<IEnumerable<Distributor>> GetDistributorsAsync(bool trackChanges)
        {
            return await FindAll(trackChanges)
                .OrderBy(d => d.DistributorName)
                .Include(d => d.Products)
                .ToListAsync();
        }
        public async Task<IEnumerable<Distributor>> GetDistributorsByNameAsync(string Name)
        {
            return await FindAll(false).Where(d => EF.Functions.Like(d.DistributorName, $"%{Name}%")).ToListAsync(); // Sử dụng LIKE
        }

        public async Task<Distributor> GetDistributorAsync(long distributorId, bool trackChanges)
        {
            return await FindByCondition(d => d.Id.Equals(distributorId), trackChanges)
                .Include(d => d.Products)
                .SingleOrDefaultAsync();
        }

        public async Task SaveChangesAsync()
        {
            await RepositoryContext.SaveChangesAsync();
        }
    }
}