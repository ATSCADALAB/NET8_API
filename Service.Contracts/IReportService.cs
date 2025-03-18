using Shared.DataTransferObjects.Report;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IReportService
    {
        Task<IEnumerable<ProductDailyReportDto>> GetProductDailyReportAsync(
            DateTime startDate,
            DateTime endDate,
            int? lineId,
            int? productInformationId);
        Task<byte[]> ExportProductDailyReportAsync(DateTime startDate, DateTime endDate, int? lineId, int? productInformationId);
        Task<IEnumerable<ProductDailyReportDto>> GetVehicleDailyReportAsync(DateTime startDate, DateTime endDate, string licensePlate = null);
        Task<byte[]> ExportVehicleDailyReportAsync(DateTime startDate, DateTime endDate, string licensePlate = null);
        Task<IEnumerable<ProductDailyReportDto>> GetIncompleteOrderShipmentReportAsync(DateTime startDate, DateTime endDate);
        Task<byte[]> ExportIncompleteOrderShipmentReportAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<AgentProductionReportDto>> GetAgentProductionReportAsync(
            int? fromYear = null, int? toYear = null, int? fromMonth = null, int? toMonth = null, int? distributorId = null, int? productInformationId = null);
        Task<byte[]> ExportAgentProductionReportAsync(
            int? fromYear = null, int? toYear = null, int? fromMonth = null, int? toMonth = null, int? distributorId = null, int? productInformationId = null);
        Task<IEnumerable<RegionProductionReportDto>> GetRegionProductionReportAsync(
        int? fromYear = null, int? toYear = null, int? fromMonth = null, int? toMonth = null,
        int? productInformationId = null, int? areaName = null);

        Task<byte[]> ExportRegionProductionReportAsync(
            int? fromYear = null, int? toYear = null, int? fromMonth = null, int? toMonth = null,
            int? productInformationId = null, int? areaName = null);
    }
}