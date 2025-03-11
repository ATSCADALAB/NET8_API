using Entities.Models;

namespace Contracts
{
    public interface IDistributorRepository
    {
        Task<IEnumerable<Distributor>> GetDistributorsAsync(bool trackChanges);

        Task<IEnumerable<Distributor>> GetDistributorsByNameAsync(string Name);
        Task<Distributor> GetDistributorAsync(long distributorId, bool trackChanges);
        void CreateDistributor(Distributor distributor);
        void DeleteDistributor(Distributor distributor);
        // Thêm phương thức AddRangeAsync để lưu hàng loạt
        Task AddRangeAsync(IEnumerable<Distributor> distributors);
    }
}