using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    internal sealed class OutboundRecordRepository : RepositoryBase<OutboundRecord>, IOutboundRecordRepository
    {
        public OutboundRecordRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<OutboundRecord>> GetAllOutboundRecordsAsync(bool trackChanges) =>
            await FindAll(trackChanges)
                .Include(ir => ir.ProductInformation)
                .OrderBy(ir => ir.OutboundDate)
                .ToListAsync();

        public async Task<OutboundRecord> GetOutboundRecordByIdAsync(int OutboundRecordId, bool trackChanges) =>
            await FindByCondition(ir => ir.Id == OutboundRecordId, trackChanges)
                .Include(ir => ir.ProductInformation)
                .SingleOrDefaultAsync();

        public async Task<IEnumerable<OutboundRecord>> GetOutboundRecordsByProductInformationIdAsync(int productInformationId, bool trackChanges) =>
            await FindByCondition(ir => ir.ProductInformationId == productInformationId, trackChanges)
                .Include(ir => ir.ProductInformation)
                .OrderBy(ir => ir.OutboundDate)
                .ToListAsync();

        public async Task<IEnumerable<OutboundRecord>> GetOutboundRecordsByDateAsync(DateTime OutboundDate, bool trackChanges) =>
            await FindByCondition(ir => ir.OutboundDate.Date == OutboundDate.Date, trackChanges)
                .Include(ir => ir.ProductInformation)
                .OrderBy(ir => ir.OutboundDate)
                .ToListAsync();

        public void CreateOutboundRecord(OutboundRecord OutboundRecord) => Create(OutboundRecord);

        public void UpdateOutboundRecord(OutboundRecord OutboundRecord) => Update(OutboundRecord);

        public void DeleteOutboundRecord(OutboundRecord OutboundRecord) => Delete(OutboundRecord);
    }
}