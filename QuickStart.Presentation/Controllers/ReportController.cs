using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects.Report;
using System.IO;

namespace QuickStart.Presentation.Controllers
{
    [Route("api/reports")]
    [ApiController]
    //[Authorize] // Bỏ comment nếu cần authentication
    public class ReportController : ControllerBase
    {
        private readonly IServiceManager _service;

        public ReportController(IServiceManager service) => _service = service;

        [HttpGet("product-daily")]
        public async Task<IActionResult> GetProductDailyReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] int? lineId = null,
            [FromQuery] int? productInformationId = null)
        {
            var reports = await _service.ReportService.GetProductDailyReportAsync(startDate, endDate, lineId, productInformationId);
            return Ok(reports);
        }
        [HttpGet("product-daily/export")]
        public async Task<IActionResult> ExportToTemplate( [FromQuery] DateTime startDate,[FromQuery] DateTime endDate,
            [FromQuery] int? lineId = null,[FromQuery] int? productInformationId = null )
        {
            try
            {
                var fileContent = await _service.ReportService.ExportProductDailyReportAsync(startDate, endDate, lineId, productInformationId);
                if (fileContent == null || fileContent.Length == 0)
                {
                    return NoContent();
                }
                return File(
                    fileContent,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"BaoCaoSanPham_{startDate:ddMMyyyy}_{endDate:ddMMyyyy}.xlsx"
                );
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Export Report Error"
                );
            }
        }
        [HttpGet("vehicle-daily")]
        public async Task<IActionResult> GetVehicleDailyReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] string vehicleNumber = null)
        {
            var reports = await _service.ReportService.GetVehicleDailyReportAsync(startDate, endDate, vehicleNumber);
            return Ok(reports);
        }

        [HttpGet("vehicle-daily/export")]
        public async Task<IActionResult> ExportVehicleDailyReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] string vehicleNumber = null)
        {
            var fileBytes = await _service.ReportService.ExportVehicleDailyReportAsync(startDate, endDate, vehicleNumber);
            if (fileBytes == null || fileBytes.Length == 0)
            {
                return NoContent();
            }
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"VehicleDailyReport_{DateTime.Now:yyyy-MM-dd}.xlsx");
        }
        [HttpGet("incomplete-order-shipment")]
        public async Task<IActionResult> GetIncompleteOrderShipmentReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var reports = await _service.ReportService.GetIncompleteOrderShipmentReportAsync(startDate, endDate);
            return Ok(reports);
        }

        [HttpGet("incomplete-order-shipment/export")]
        public async Task<IActionResult> ExportIncompleteOrderShipmentReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var fileBytes = await _service.ReportService.ExportIncompleteOrderShipmentReportAsync(startDate, endDate);
            if (fileBytes == null || fileBytes.Length == 0)
            {
                return NoContent();
            }
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"IncompleteOrderShipmentReport_{DateTime.Now:yyyy-MM-dd}.xlsx");
        }
        [HttpGet("distributor-production")]
        public async Task<IActionResult> GetDistributorProductionReport(
    [FromQuery] int? fromYear = null,
    [FromQuery] int? toYear = null,
    [FromQuery] int? fromMonth = null,
    [FromQuery] int? toMonth = null,
    [FromQuery] int? distributorId = null,
    [FromQuery] int? productInformationId = null)
        {
            var reports = await _service.ReportService.GetAgentProductionReportAsync(fromYear, toYear, fromMonth, toMonth, distributorId, productInformationId);
            return Ok(reports);
        }

        [HttpGet("distributor-production/export")]
        public async Task<IActionResult> ExportDistributorProductionReport(
            [FromQuery] int? fromYear = null,
            [FromQuery] int? toYear = null,
            [FromQuery] int? fromMonth = null,
            [FromQuery] int? toMonth = null,
            [FromQuery] int? distributorId = null,
            [FromQuery] int? productInformationId = null)
        {
            var fileBytes = await _service.ReportService.ExportAgentProductionReportAsync(fromYear, toYear, fromMonth, toMonth, distributorId, productInformationId);
            if (fileBytes == null || fileBytes.Length == 0)
            {
                return NoContent();
            }
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DistributorProductionReport.xlsx");
        }
        [HttpGet("region-production")]
        public async Task<IActionResult> GetRegionProductionReport(
        [FromQuery] int? fromYear = null,
        [FromQuery] int? toYear = null,
        [FromQuery] int? fromMonth = null,
        [FromQuery] int? toMonth = null,
        [FromQuery] int? productInformationId = null,
        [FromQuery] int? areaId = null)
        {
            var reports = await _service.ReportService.GetRegionProductionReportAsync(fromYear, toYear, fromMonth, toMonth, productInformationId, areaId);
            if (reports == null)
            {
                return NotFound("No data available for Region Production Report.");
            }
            return Ok(reports);
        }

        [HttpGet("region-production/export")]
        public async Task<IActionResult> ExportRegionProductionReport(
            [FromQuery] int? fromYear = null,
            [FromQuery] int? toYear = null,
            [FromQuery] int? fromMonth = null,
            [FromQuery] int? toMonth = null,
            [FromQuery] int? productInformationId = null,
            [FromQuery] int? areaId = null)
        {
            var fileBytes = await _service.ReportService.ExportRegionProductionReportAsync(fromYear, toYear, fromMonth, toMonth, productInformationId, areaId);
            if (fileBytes == null)
            {
                return NotFound("No data available for export.");
            }
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "RegionProductionReport.xlsx");
        }
    }
}