using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IAreaRepository
    {
        // Lấy tất cả khu vực
        Task<IEnumerable<Area>> GetAllAreasAsync(bool trackChanges);

        // Lấy khu vực theo ID
        Task<Area> GetAreaByIdAsync(int areaId, bool trackChanges);

        // Lấy khu vực theo mã khu vực
        Task<Area> GetAreaByCodeAsync(string areaCode, bool trackChanges);

        // Tạo khu vực mới
        void CreateArea(Area area);

        // Cập nhật thông tin khu vực
        void UpdateArea(Area area);

        // Xóa khu vực
        void DeleteArea(Area area);
    }
}