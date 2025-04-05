using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    internal sealed class InboundRecordRepository : RepositoryBase<InboundRecord>, IInboundRecordRepository
    {
        public InboundRecordRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<InboundRecord>> GetAllInboundRecordsAsync(bool trackChanges) =>
            await FindAll(trackChanges)
                .Include(ir => ir.ProductInformation)
                .Include(ir => ir.CreatedBy)
                .Include(ir => ir.UpdatedBy)
                .OrderBy(ir => ir.InboundDate)
                .ToListAsync();

        public async Task<InboundRecord> GetInboundRecordByIdAsync(int inboundRecordId, bool trackChanges) =>
            await FindByCondition(ir => ir.Id == inboundRecordId, trackChanges)
                .Include(ir => ir.ProductInformation)
                .Include(ir => ir.CreatedBy)
                .Include(ir => ir.UpdatedBy)
                .SingleOrDefaultAsync();

        public async Task<IEnumerable<InboundRecord>> GetInboundRecordsByProductInformationIdAsync(int productInformationId, bool trackChanges) =>
            await FindByCondition(ir => ir.ProductInformationId == productInformationId, trackChanges)
                .Include(ir => ir.ProductInformation)
                .Include(ir => ir.CreatedBy)
                .Include(ir => ir.UpdatedBy)
                .OrderBy(ir => ir.InboundDate)
                .ToListAsync();

        public async Task<IEnumerable<InboundRecord>> GetInboundRecordsByDateAsync(DateTime inboundDate, bool trackChanges) =>
            await FindByCondition(ir => ir.InboundDate.Date == inboundDate.Date, trackChanges)
                .Include(ir => ir.ProductInformation)
                .Include(ir => ir.CreatedBy)
                .Include(ir => ir.UpdatedBy)
                .OrderBy(ir => ir.InboundDate)
                .ToListAsync();

        public void CreateInboundRecord(InboundRecord inboundRecord) => Create(inboundRecord);

        public void UpdateInboundRecord(InboundRecord inboundRecord) => Update(inboundRecord);

        public void DeleteInboundRecord(InboundRecord inboundRecord) => Delete(inboundRecord);
    }
}