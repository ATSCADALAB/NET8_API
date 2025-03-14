using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    internal sealed class AreaRepository : RepositoryBase<Area>, IAreaRepository
    {
        public AreaRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<Area>> GetAllAreasAsync(bool trackChanges) =>
            await FindAll(trackChanges)
                .OrderBy(a => a.AreaCode)
                .ToListAsync();

        public async Task<Area> GetAreaByIdAsync(int areaId, bool trackChanges) =>
            await FindByCondition(a => a.Id == areaId, trackChanges)
                .SingleOrDefaultAsync();

        public async Task<Area> GetAreaByCodeAsync(string areaCode, bool trackChanges) =>
            await FindByCondition(a => a.AreaCode == areaCode, trackChanges)
                .SingleOrDefaultAsync();

        public void CreateArea(Area area) => Create(area);

        public void UpdateArea(Area area) => Update(area);

        public void DeleteArea(Area area) => Delete(area);
    }
}