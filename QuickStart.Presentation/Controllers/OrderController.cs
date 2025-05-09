﻿using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuickStart.Presentation.ActionFilters;
using Service.Contracts;
using Shared.DataTransferObjects.Order;
using Shared.DataTransferObjects.OrderDetail;

namespace QuickStart.Presentation.Controllers
{
    [Route("api/orders")]
    [ApiController]
    //[Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IServiceManager _service;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderController(IServiceManager service, IHttpContextAccessor contextAccessor)
        {
            _service = service;
            _httpContextAccessor = contextAccessor;
        }

        [HttpGet]
        //[AuthorizePermission("Orders", "View")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _service.OrderService.GetAllOrdersAsync(trackChanges: false);
            return Ok(orders);
        }
        [HttpGet("template")]
        public IActionResult DownloadOrderTemplate()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "Order.xlsx");
            if (!System.IO.File.Exists(filePath))
                return NotFound("Template file not found.");

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Order.xlsx");
        }
        [HttpGet("{orderId:guid}", Name = "GetOrderById")]
        //[AuthorizePermission("Orders", "View")]
        public async Task<IActionResult> GetOrder(Guid orderId)
        {
            var order = await _service.OrderService.GetOrderAsync(orderId, trackChanges: false);
            return Ok(order);
        }

        [HttpGet("code/{orderCode}")]
        //[AuthorizePermission("Orders", "View")]
        public async Task<IActionResult> GetOrderByCode(string orderCode)
        {
            var order = await _service.OrderService.GetOrderByCodeAsync(orderCode, trackChanges: false);
            return Ok(order);
        }

        [HttpGet("by-distributor/{distributorId:int}")]
        //[AuthorizePermission("Orders", "View")]
        public async Task<IActionResult> GetOrdersByDistributor(int distributorId)
        {
            var orders = await _service.OrderService.GetOrdersByDistributorAsync(distributorId, trackChanges: false);
            return Ok(orders);
        }

        [HttpGet("by-export-date")]
        //[AuthorizePermission("Orders", "View")]
        public async Task<IActionResult> GetOrdersByExportDate([FromQuery] DateTime exportDate)
        {
            var orders = await _service.OrderService.GetOrdersByExportDateAsync(exportDate, trackChanges: false);
            return Ok(orders);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [AuthorizePermission("Orders", "Create")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderForCreationDto order)
        {
            var createdOrder = await _service.OrderService.CreateOrderAsync(order, _httpContextAccessor);
            return CreatedAtRoute("GetOrderById", new { orderId = createdOrder.Id }, createdOrder);
        }

        [HttpPut("{orderId:guid}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [AuthorizePermission("Orders", "Update")]
        public async Task<IActionResult> UpdateOrder(Guid orderId, [FromBody] OrderForUpdateDto orderForUpdate)
        {
            await _service.OrderService.UpdateOrderAsync(orderId, orderForUpdate, _httpContextAccessor, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{orderId:guid}")]
        [AuthorizePermission("Orders", "Delete")]
        public async Task<IActionResult> DeleteOrder(Guid orderId)
        {
            await _service.OrderService.DeleteOrderAsync(orderId, trackChanges: false);
            return NoContent();
        }
        //[HttpPost("import")]
        //[AuthorizePermission("Orders", "Create")]
        //public async Task<IActionResult> ImportOrders(IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest("No file uploaded.");

        //    try
        //    {
        //        using (var stream = new MemoryStream())
        //        {
        //            await file.CopyToAsync(stream);
        //            using (var workbook = new XLWorkbook(stream))
        //            {
        //                var worksheet = workbook.Worksheet(1);
        //                if (worksheet == null)
        //                    return BadRequest("Excel file has no valid worksheet.");

        //                var rowCount = worksheet.RowsUsed().Count();
        //                if (rowCount < 2)
        //                    return BadRequest("Excel file is empty or has no data rows.");

        //                var errors = new List<string>();
        //                var skippedRows = new List<string>(); // Ghi lại các dòng bị bỏ qua vì trùng
        //                int successCount = 0;

        //                var distributors = await _service.DistributorService.GetAllDistributorsAsync(trackChanges: false);
        //                var distributorDict = distributors.ToDictionary(d => d.DistributorName, d => d.Id);

        //                var products = await _service.ProductInformationService.GetAllProductInformationsAsync(trackChanges: false);
        //                var productDict = products.ToDictionary(p => p.ProductCode, p => p.Id);

        //                var existingOrders = await _service.OrderService.GetAllOrdersAsync(trackChanges: false);
        //                var existingOrderDetails = await _service.OrderDetailService.GetAllOrderDetailsAsync(trackChanges: false);

        //                for (int row = 4; row <= rowCount + 2; row++)
        //                {
        //                    try
        //                    {
        //                        var orderCode = worksheet.Cell(row, 2).GetValue<string>()?.Trim();
        //                        if (string.IsNullOrWhiteSpace(orderCode))
        //                            throw new Exception("OrderCode is missing.");

        //                        var exportDate = worksheet.Cell(row, 1).GetValue<DateTime>();
        //                        var drivernumber = worksheet.Cell(row, 3).GetValue<int>();
        //                        var vehicleNumber = worksheet.Cell(row, 9).GetValue<string>()?.Trim();
        //                        var driverName = worksheet.Cell(row, 12).GetValue<string>()?.Trim().Replace("_x000D_", "").Trim();
        //                        var driverPhoneNumber = worksheet.Cell(row, 13).GetValue<string>()?.Trim();
        //                        var distributorName = worksheet.Cell(row, 14).GetValue<string>()?.Trim();
        //                        var productCode = worksheet.Cell(row, 4).GetValue<string>()?.Trim();
        //                        var requestedWeight = worksheet.Cell(row, 6).GetValue<decimal>();
        //                        var requestedUnits = worksheet.Cell(row, 7).GetValue<int>();
        //                        var manufactureDateCell = worksheet.Cell(row, 8);

        //                        // Xử lý ManufactureDate: Nếu ô trống hoặc không phải định dạng ngày, đặt là null
        //                        DateTime? manufactureDate = null;
        //                        if (!manufactureDateCell.IsEmpty())
        //                        {
        //                            try
        //                            {
        //                                manufactureDate = manufactureDateCell.GetValue<DateTime>();
        //                            }
        //                            catch (Exception)
        //                            {
        //                                manufactureDate = null; // Nếu không parse được, đặt là null
        //                            }
        //                        }

        //                        if (exportDate == default || string.IsNullOrWhiteSpace(vehicleNumber) ||
        //                            string.IsNullOrWhiteSpace(driverName) || string.IsNullOrWhiteSpace(driverPhoneNumber) ||
        //                            string.IsNullOrWhiteSpace(distributorName))
        //                            throw new Exception("Missing required Order fields.");

        //                        if (!distributorDict.TryGetValue(distributorName, out int distributorId))
        //                            throw new Exception($"Distributor '{distributorName}' not found.");

        //                        if (!productDict.TryGetValue(productCode, out int productInformationId))
        //                            throw new Exception($"Product '{productCode}' not found.");

        //                        // Kiểm tra trùng lặp, so sánh với manufactureDate là null nếu cần
        //                        var isDuplicate = existingOrders.Any(o =>
        //                            o.OrderCode == orderCode &&
        //                            o.ExportDate == exportDate &&
        //                            o.VehicleNumber == vehicleNumber &&
        //                            o.DriverName == driverName &&
        //                            o.DistributorId == distributorId &&
        //                            existingOrderDetails.Any(od =>
        //                                od.OrderId == o.Id &&
        //                                od.ProductInformationId == productInformationId &&
        //                                (manufactureDate.HasValue
        //                                    ? od.ManufactureDate == manufactureDate.Value
        //                                    : (od.ManufactureDate == null || od.ManufactureDate == DateTime.MinValue)) &&
        //                                od.RequestedUnits == requestedUnits));

        //                        if (isDuplicate)
        //                        {
        //                            skippedRows.Add($"Row {row}: Skipped due to duplicate data (OrderCode: {orderCode}, ExportDate: {exportDate}, ProductCode: {productCode}, ManufactureDate: {manufactureDate}, VehicleNumber: {vehicleNumber}, DriverName: {driverName}, Distributor: {distributorName}, RequestedUnits: {requestedUnits})");
        //                            continue; // Bỏ qua dòng này
        //                        }

        //                        // Tạo Order nếu không trùng
        //                        var order = new OrderForCreationDto
        //                        {
        //                            OrderCode = orderCode,
        //                            ExportDate = exportDate,
        //                            DriverNumber = drivernumber,
        //                            VehicleNumber = vehicleNumber,
        //                            DriverName = driverName,
        //                            DriverPhoneNumber = driverPhoneNumber,
        //                            Status = 0,
        //                            DistributorId = distributorId
        //                        };

        //                        var createdOrder = await _service.OrderService.CreateOrderAsync(order);

        //                        var orderDetail = new OrderDetailForCreationDto
        //                        {
        //                            OrderId = createdOrder.Id,
        //                            ProductInformationId = productInformationId,
        //                            RequestedUnits = requestedUnits,
        //                            RequestedWeight = requestedWeight,
        //                            ManufactureDate = manufactureDate, // Sử dụng DateTime? để hỗ trợ null
        //                            DefectiveUnits = 0,
        //                            DefectiveWeight = 0,
        //                            ReplacedUnits = 0,
        //                            ReplacedWeight = 0
        //                        };

        //                        await _service.OrderDetailService.CreateOrderDetailAsync(orderDetail);
        //                        successCount++;
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        errors.Add($"Row {row}: {ex.Message}");
        //                    }
        //                }

        //                var result = new
        //                {
        //                    SuccessCount = successCount,
        //                    SkippedCount = skippedRows.Count,
        //                    SkippedRows = skippedRows,
        //                    Errors = errors
        //                };

        //                if (successCount == 0 && skippedRows.Count == 0)
        //                    return BadRequest($"No valid orders imported:\n{string.Join("\n", errors)}");

        //                return Ok(result);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Error importing orders: {ex.Message} - StackTrace: {ex.StackTrace}");
        //    }
        //}
        [HttpGet("by-filter")]
        //[AuthorizePermission("Orders", "View")]
        public async Task<IActionResult> GetOrdersByFilter(
            [FromQuery] DateTime startDate, // Bắt buộc
            [FromQuery] DateTime endDate,   // Bắt buộc
            [FromQuery] int? distributorId, // Tùy chọn
            [FromQuery] int? areaId,        // Tùy chọn
            [FromQuery] int? productInformationId, // Tùy chọn
            [FromQuery] int? status)        // Tùy chọn
        {
            if (startDate == default || endDate == default)
            {
                return BadRequest("Start date and end date are required.");
            }

            if (startDate > endDate)
            {
                return BadRequest("Start date must be less than or equal to end date.");
            }

            var orders = await _service.OrderService.GetOrdersByFilterAsync(
                startDate,
                endDate,
                distributorId,
                areaId,
                productInformationId,
                status,
                trackChanges: false);

            return Ok(orders);
        }
        [HttpPost("import")]
        [AuthorizePermission("Orders", "Create")]
        public async Task<IActionResult> ImportOrders(IFormFile file)
        {
            var orders = await _service.OrderService.ImportOrdersFromExcelAsync(file, _httpContextAccessor);
            return Ok(orders);
        }
    }
}