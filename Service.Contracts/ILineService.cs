using Shared.DataTransferObjects.Line;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface ILineService
    {
        Task<IEnumerable<LineDto>> GetAllLinesAsync(bool trackChanges);
        Task<LineDto> GetLineAsync(int lineId, bool trackChanges);
        Task<LineDto> GetLineByNumberAsync(int lineNumber, bool trackChanges);
        Task<LineDto> CreateLineAsync(LineForCreationDto line);
        Task UpdateLineAsync(int lineId, LineForUpdateDto lineForUpdate, bool trackChanges);
        Task DeleteLineAsync(int lineId, bool trackChanges);
    }
}