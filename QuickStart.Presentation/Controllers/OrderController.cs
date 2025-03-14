using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects.Order;
using System.Text.RegularExpressions;
namespace QuickStart.Presentation.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IServiceManager _service;

        public OrderController(IServiceManager service) => _service = service;
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] string productInfo, [FromQuery] bool status)
        //{
        //    try
        //    {
        //        var orders = await _service.OrderService.GetOrdersByFilters(startDate, endDate, productInfo, status);
        //        if (orders == null)
        //        {
        //            return NotFound("No orders found.");
        //        }
        //        return Ok(orders);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving orders: {ex.Message}");
        //    }
        //}
        //Lấy tất cả đơn hàng
        [HttpGet("template")]
        public IActionResult DownloadProductInformationTemplate()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "Order.xlsx");
            if (!System.IO.File.Exists(filePath))
                return NotFound("Template file not found.");

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Order.xlsx");
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
        {
            try
            {
                var orders = await _service.OrderService.GetAllOrdersAsync();
                if (orders == null)
                {
                    return NotFound("No orders found.");
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving orders: {ex.Message}");
            }
        }

        // Lấy chi tiết một đơn hàng theo ID
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(Guid orderId)
        {
            try
            {
                var order = await _service.OrderService.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    return NotFound($"Order with ID {orderId} not found.");
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving order: {ex.Message}");
            }
        }
        // Lấy chi tiết một đơn hàng theo mã đơn hàng
        [HttpGet("ByOrderCode/{orderCode}")]
        public async Task<ActionResult<OrderDto>> GetOrderByOrderCode(string orderCode)
        {
            try
            {
                var order = await _service.OrderService.GetOrderByOrderCodeAsync(orderCode);
                if (order == null)
                {
                    return NotFound($"Order with Order code {orderCode} not found.");
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving order: {ex.Message}");
            }
        }
        // Tạo đơn hàng mới
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] OrderForCreationDto orderForCreationDto)
        {
            try
            {
                if (orderForCreationDto == null)
                {
                    return BadRequest("Order data is null.");
                }

                var createdOrder = await _service.OrderService.CreateOrderAsync(orderForCreationDto);

                return CreatedAtAction(nameof(GetOrderById), new { orderId = createdOrder.Id }, createdOrder);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error creating order: {ex.Message}");
            }
        }

        // Cập nhật đơn hàng
        [HttpPut("{orderId}")]
        public async Task<ActionResult<OrderDto>> UpdateOrder(Guid orderId, [FromBody] OrderForUpdateDto orderForUpdateDto)
        {
            try
            {
                if (orderForUpdateDto == null)
                {
                    return BadRequest("Order update data is null.");
                }

                var updatedOrder = await _service.OrderService.UpdateOrderAsync(orderId, orderForUpdateDto);

                if (updatedOrder == null)
                {
                    return NotFound($"Order with ID {orderId} not found.");
                }

                return Ok(updatedOrder);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating order: {ex.Message}");
            }
        }
        // Xoá đơn hàng 
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            await _service.OrderService.DeleteOrderAsync(id, trackChanges: false);

            return NoContent();
        }
        [HttpPatch("{id:guid}/status")]
        public async Task<ActionResult> UpdateOrderStatus(Guid id, [FromBody] int newStatus)
        {
            try
            {
                if (newStatus < 0 || newStatus > 3) // Giả sử Status chỉ từ 0-2
                    return BadRequest("Invalid status value. Status must be between 0 and 2.");

                await _service.OrderService.UpdateStatusAsync(id, 2);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating order status: {ex.Message}");
            }
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportOrders(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty or not provided.");

            try
            {
                // Lấy toàn bộ thông tin productInformation và distributor để gán dữ liệu
                var listProductInformation = await _service.ProductInformationService.GetAllProductInformationsAsync(false);
                var listDistributor = await _service.DistributorService.GetAllDistributorsAsync(false);
                var existingOrders = await _service.OrderService.GetAllOrdersAsync(); // Lấy danh sách order hiện có

                using (var stream = file.OpenReadStream())
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1); // Lấy sheet đầu tiên
                    var ordersCreation = new List<OrderForCreationDto>();
                    var errors = new List<string>(); // Lưu lỗi
                    int successfulImports = 0; // Số dòng import thành công
                    int duplicateRows = 0; // Số dòng bị trùng lặp

                    var currentRow = 4;
                    int maxRows = worksheet.LastRowUsed().RowNumber();

                    if (maxRows < 4)
                        return BadRequest("Excel file has no data rows.");

                    while (currentRow <= maxRows)
                    {
                        var productCodeCell = worksheet.Cell(currentRow, 4); // Cột 'Mã Hàng'
                        var distributorCell = worksheet.Cell(currentRow, 14); // Cột 'Tên Đại Lý'

                        if (!productCodeCell.IsEmpty() && !distributorCell.IsEmpty())
                        {
                            var productCode = productCodeCell.GetString()?.Trim();
                            var distributorName = distributorCell.GetString()?.Trim();

                            var productInfo = listProductInformation.FirstOrDefault(x => x.ProductCode.Equals(productCode, StringComparison.OrdinalIgnoreCase));
                            var distributorInfo = listDistributor.FirstOrDefault(x => x.DistributorName.Equals(distributorName, StringComparison.OrdinalIgnoreCase));

                            if (productInfo != null && distributorInfo != null)
                            {
                                // Lấy và làm sạch DriverName, loại bỏ _x000D_\n và các ký tự xuống dòng
                                var driverNameRaw = worksheet.Cell(currentRow, 12).GetString() ?? string.Empty;
                                var driverName = driverNameRaw.Trim()
                                    .Replace("_x000D_\n", "")
                                    .Replace("_x000D_", "")
                                    .Replace("\r\n", "")
                                    .Replace("\n", "")
                                    .Replace("\r", "")
                                    .Replace("\t", "");

                                var order = new OrderForCreationDto
                                {
                                    ProductInformationID = productInfo.Id,
                                    ExportDate = DateTime.TryParse(worksheet.Cell(currentRow, 1).GetString(), out var exportDate) ? exportDate : (DateTime?)null,
                                    Code = worksheet.Cell(currentRow, 2).GetString()?.Trim(),
                                    QuantityVehicle = int.TryParse(worksheet.Cell(currentRow, 3).GetString(), out var qtyVeh) ? qtyVeh : 0,
                                    WeightOrder = decimal.TryParse(worksheet.Cell(currentRow, 6).GetString(), out var cmtp) ? cmtp : 0,
                                    UnitOrder = worksheet.Cell(currentRow, 7).GetString()?.Trim(),
                                    ManufactureDate = DateTime.TryParse(worksheet.Cell(currentRow, 8).GetString(), out var manufactureDate) ? manufactureDate : (DateTime?)null,
                                    VehicleNumber = worksheet.Cell(currentRow, 9).GetString()?.Trim(),
                                    ContainerNumber = int.TryParse(worksheet.Cell(currentRow, 10).GetString(), out var contNumber) ? contNumber : 0,
                                    SealNumber = int.TryParse(worksheet.Cell(currentRow, 11).GetString(), out var sealNumber) ? sealNumber : 0,
                                    DriverName = driverName,
                                    DriverPhoneNumber = worksheet.Cell(currentRow, 13).GetString()?.Trim(),
                                    DistributorID = distributorInfo.Id
                                };

                                // Kiểm tra trùng lặp với danh sách order hiện có
                                bool isDuplicate = existingOrders.Any(o =>
                                    o.Code == order.Code &&
                                    o.ExportDate == order.ExportDate &&
                                    o.VehicleNumber == order.VehicleNumber);

                                if (!isDuplicate)
                                {
                                    ordersCreation.Add(order);
                                    successfulImports++; // Tăng số dòng import thành công
                                }
                                else
                                {
                                    duplicateRows++; // Tăng số dòng bị trùng lặp
                                }
                            }
                            else
                            {
                                duplicateRows++;
                                errors.Add($"Row {currentRow}: ProductCode '{productCode}' or Distributor '{distributorName}' not found.");
                            }
                        }

                        currentRow++;
                    }


                    var importedOrders = await _service.OrderService.ImportOrdersAsync(ordersCreation);

                    // Trả về báo cáo chi tiết
                    var result = new
                    {
                        SuccessfulImports = successfulImports,
                        DuplicateRows = duplicateRows,
                    };

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error importing orders: {ex.Message}");
            }
        }

    }
}
