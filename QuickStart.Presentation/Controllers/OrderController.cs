using ClosedXML.Excel;
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