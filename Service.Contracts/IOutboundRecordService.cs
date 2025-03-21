using Shared.DataTransferObjects.OutboundRecord;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IOutboundRecordService
    {
        Task<IEnumerable<OutboundRecordDto>> GetAllOutboundRecordsAsync(bool trackChanges);
        Task<OutboundRecordDto> GetOutboundRecordAsync(int outboundRecordId, bool trackChanges);
        Task<IEnumerable<OutboundRecordDto>> GetOutboundRecordsByProductInformationAsync(int productInformationId, bool trackChanges);
        Task<IEnumerable<OutboundRecordDto>> GetOutboundRecordsByDateAsync(DateTime outboundDate, bool trackChanges);
        Task<OutboundRecordDto> CreateOutboundRecordAsync(OutboundRecordForCreationDto outboundRecord);
    }
}