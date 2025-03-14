using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    internal sealed class LineRepository : RepositoryBase<Line>, ILineRepository
    {
        public LineRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<Line>> GetAllLinesAsync(bool trackChanges) =>
            await FindAll(trackChanges)
                .OrderBy(l => l.LineNumber)
                .ToListAsync();

        public async Task<Line> GetLineByIdAsync(int lineId, bool trackChanges) =>
            await FindByCondition(l => l.Id == lineId, trackChanges)
                .SingleOrDefaultAsync();

        public async Task<Line> GetLineByNumberAsync(int lineNumber, bool trackChanges) =>
            await FindByCondition(l => l.LineNumber == lineNumber, trackChanges)
                .SingleOrDefaultAsync();

        public void CreateLine(Line line) => Create(line);

        public void UpdateLine(Line line) => Update(line);

        public void DeleteLine(Line line) => Delete(line);
    }
}