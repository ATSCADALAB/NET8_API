using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ILineRepository
    {
        // Lấy tất cả dây chuyền
        Task<IEnumerable<Line>> GetAllLinesAsync(bool trackChanges);

        // Lấy dây chuyền theo ID
        Task<Line> GetLineByIdAsync(int lineId, bool trackChanges);

        // Lấy dây chuyền theo số hiệu
        Task<Line> GetLineByNumberAsync(int lineNumber, bool trackChanges);

        // Tạo dây chuyền mới
        void CreateLine(Line line);

        // Cập nhật thông tin dây chuyền
        void UpdateLine(Line line);

        // Xóa dây chuyền
        void DeleteLine(Line line);
    }
}