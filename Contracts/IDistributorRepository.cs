using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IDistributorRepository
    {
        // Lấy tất cả đại lý
        Task<IEnumerable<Distributor>> GetAllDistributorsAsync(bool trackChanges);

        // Lấy đại lý theo ID
        Task<Distributor> GetDistributorByIdAsync(int distributorId, bool trackChanges);

        // Lấy đại lý theo mã đại lý
        Task<Distributor> GetDistributorByCodeAsync(string distributorCode, bool trackChanges);

        // Lấy danh sách đại lý theo khu vực
        Task<IEnumerable<Distributor>> GetDistributorsByAreaAsync(int areaId, bool trackChanges);

        // Tạo đại lý mới
        void CreateDistributor(Distributor distributor);

        // Cập nhật thông tin đại lý
        void UpdateDistributor(Distributor distributor);

        // Xóa đại lý
        void DeleteDistributor(Distributor distributor);
    }
}