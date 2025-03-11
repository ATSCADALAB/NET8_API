using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects.Order;
namespace QuickStart.Presentation.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IServiceManager _service;

        public OrderController(IServiceManager service) => _service = service;
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] string productInfo, [FromQuery] bool status)
        {
            try
            {
                var orders = await _service.OrderService.GetOrdersByFilters(startDate, endDate, productInfo, status);
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
        // Lấy tất cả đơn hàng
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
        //{
        //    try
        //    {
        //        var orders = await _service.OrderService.GetAllOrdersAsync();
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

        // Lấy chi tiết một đơn hàng theo ID
        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(Guid orderId)
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


        // Nhập khẩu đơn hàng từ file (ví dụ từ Excel)
        [HttpPost("import")]
        public async Task<IActionResult> ImportOrders(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty or not provided.");

            try
            {
                //Lấy toàn bộ thông tin productInformation để gán dữ liệu
                var listProductInformation = await _service.ProductInformationService.GetAllProductInformationsAsync(false);
                //Lấy toàn bộ thông tin distributor để gán dữ liệu
                var listDistributor = await _service.DistributorService.GetAllDistributorsAsync(false);
                using (var stream = file.OpenReadStream())
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1); // Lấy sheet đầu tiên
                    var orders = new List<OrderForCreationDto>();

                    // Bắt đầu đọc từ dòng 4 (bỏ qua header)
                    var currentRow = 4;
                    // Giới hạn tối đa số dòng cần đọc, ví dụ: 1000 (tùy thuộc vào dữ liệu thực tế của bạn)
                    int maxRows = worksheet.LastRowUsed().RowNumber(); // Xác định dòng cuối cùng được sử dụng

                    while (currentRow <= maxRows)
                    {
                        var productCodeCell = worksheet.Cell(currentRow, 4); // Lấy giá trị 'Mã Hàng' trong file import
                        var distributorCell = worksheet.Cell(currentRow, 14); // Lấy giá trị 'Mã Hàng' trong file import
                        // Kiểm tra 
                        if (!productCodeCell.IsEmpty())
                        {
                            var productCode = productCodeCell.GetString();
                            var distributorName = distributorCell.GetString();
                            var productInfo = listProductInformation.FirstOrDefault(x => x.ProductCode.Equals(productCode, StringComparison.OrdinalIgnoreCase));
                            var distributorInfo = listDistributor.FirstOrDefault(x => x.DistributorName.Equals(distributorName, StringComparison.OrdinalIgnoreCase));

                            // Kiểm tra nếu cả hai thông tin đều không null
                            if (productInfo != null && distributorInfo != null)
                            {
                                var order = new OrderForCreationDto
                                {
                                    ProductInformationID = productInfo.Id, // Thông tin của sản phẩm
                                    ExportDate = DateTime.TryParse(worksheet.Cell(currentRow, 1).GetString(), out var exportDate) ? exportDate : (DateTime?)null, // Ngày xuất hàng
                                    Code = worksheet.Cell(currentRow, 2).GetString(), // Số phiếu XK
                                    QuantityVehicle = int.TryParse(worksheet.Cell(currentRow, 3).GetString(), out var QtyVeh) ? QtyVeh : 0, // Số tài xe
                                    WeightOrder = decimal.TryParse(worksheet.Cell(currentRow, 6).GetString(), out var cmtp) ? cmtp : 0, // Số Lượng Order (Kg)
                                    UnitOrder = worksheet.Cell(currentRow, 7).GetString(), // Số lượng Order (Bao)
                                    ManufactureDate = DateTime.TryParse(worksheet.Cell(currentRow, 8).GetString(), out var manufactureDate) ? manufactureDate : (DateTime?)null, // Ngày SX
                                    VehicleNumber = worksheet.Cell(currentRow, 9).GetString(), // Biển số xe lấy hàng
                                    ContainerNumber = int.TryParse(worksheet.Cell(currentRow, 10).GetString(), out var contNumber) ? contNumber : 0, // Số Cont
                                    SealNumber = int.TryParse(worksheet.Cell(currentRow, 11).GetString(), out var sealNumber) ? sealNumber : 0, // Số Seal
                                    DriverName = worksheet.Cell(currentRow, 12).GetString().Trim(), // Tên TX lấy hàng
                                    DriverPhoneNumber = worksheet.Cell(currentRow, 13).GetString().Trim(), // SĐT TX lấy hàng
                                    DistributorID = distributorInfo.Id, // Nhà Phân Phối
                                };

                                // Kiểm tra xem Order có bị trùng không
                                bool isDuplicate = orders.Any(o =>
                                    o.Code == order.Code &&
                                    o.ExportDate == order.ExportDate &&
                                    o.VehicleNumber == order.VehicleNumber &&
                                    o.ManufactureDate == order.ManufactureDate);

                                if (!isDuplicate)
                                {
                                    orders.Add(order); // Chỉ thêm nếu không trùng
                                }
                                //orders.Add(order); // Chỉ thêm vào danh sách nếu hợp lệ
                            }
                        }

                        // Tăng dòng hiện tại lên để kiểm tra dòng tiếp theo
                        currentRow++;
                    }
                    var importedOrders = await _service.OrderService.ImportOrdersAsync(orders);
                    return Ok(importedOrders);
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi, ví dụ: log lỗi hoặc trả về BadRequest
                return BadRequest($"Error importing orders: {ex.Message}");
            }
        }
       
    }
}
