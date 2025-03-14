using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickStart.Presentation.ActionFilters;
using Service.Contracts;
using Shared.DataTransferObjects.OrderDetail;

namespace QuickStart.Presentation.Controllers
{
    [Route("api/order-details")]
    [ApiController]
    //[Authorize]
    public class OrderDetailController : ControllerBase
    {
        private readonly IServiceManager _service;

        public OrderDetailController(IServiceManager service) => _service = service;

        [HttpGet]
        [AuthorizePermission("OrderDetails", "View")]
        public async Task<IActionResult> GetAllOrderDetails()
        {
            var orderDetails = await _service.OrderDetailService.GetAllOrderDetailsAsync(trackChanges: false);
            return Ok(orderDetails);
        }

        [HttpGet("{orderDetailId:int}", Name = "GetOrderDetailById")]
        [AuthorizePermission("OrderDetails", "View")]
        public async Task<IActionResult> GetOrderDetail(int orderDetailId)
        {
            var orderDetail = await _service.OrderDetailService.GetOrderDetailAsync(orderDetailId, trackChanges: false);
            return Ok(orderDetail);
        }

        [HttpGet("by-order/{orderId:guid}")]
        [AuthorizePermission("OrderDetails", "View")]
        public async Task<IActionResult> GetOrderDetailsByOrder(Guid orderId)
        {
            var orderDetails = await _service.OrderDetailService.GetOrderDetailsByOrderAsync(orderId, trackChanges: false);
            return Ok(orderDetails);
        }

        [HttpGet("by-product/{productInformationId:int}")]
        [AuthorizePermission("OrderDetails", "View")]
        public async Task<IActionResult> GetOrderDetailsByProduct(int productInformationId)
        {
            var orderDetails = await _service.OrderDetailService.GetOrderDetailsByProductAsync(productInformationId, trackChanges: false);
            return Ok(orderDetails);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [AuthorizePermission("OrderDetails", "Create")]
        public async Task<IActionResult> CreateOrderDetail([FromBody] OrderDetailForCreationDto orderDetail)
        {
            var createdOrderDetail = await _service.OrderDetailService.CreateOrderDetailAsync(orderDetail);
            return CreatedAtRoute("GetOrderDetailById", new { orderDetailId = createdOrderDetail.Id }, createdOrderDetail);
        }

        [HttpPut("{orderDetailId:int}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [AuthorizePermission("OrderDetails", "Update")]
        public async Task<IActionResult> UpdateOrderDetail(int orderDetailId, [FromBody] OrderDetailForUpdateDto orderDetailForUpdate)
        {
            await _service.OrderDetailService.UpdateOrderDetailAsync(orderDetailId, orderDetailForUpdate, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{orderDetailId:int}")]
        [AuthorizePermission("OrderDetails", "Delete")]
        public async Task<IActionResult> DeleteOrderDetail(int orderDetailId)
        {
            await _service.OrderDetailService.DeleteOrderDetailAsync(orderDetailId, trackChanges: false);
            return NoContent();
        }
    }
}