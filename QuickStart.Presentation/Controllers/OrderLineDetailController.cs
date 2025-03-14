using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickStart.Presentation.ActionFilters;
using Service.Contracts;
using Shared.DataTransferObjects.OrderLineDetail;

namespace QuickStart.Presentation.Controllers
{
    [Route("api/order-line-details")]
    [ApiController]
    //[Authorize]
    public class OrderLineDetailController : ControllerBase
    {
        private readonly IServiceManager _service;

        public OrderLineDetailController(IServiceManager service) => _service = service;

        [HttpGet]
        [AuthorizePermission("OrderLineDetails", "View")]
        public async Task<IActionResult> GetAllOrderLineDetails()
        {
            var orderLineDetails = await _service.OrderLineDetailService.GetAllOrderLineDetailsAsync(trackChanges: false);
            return Ok(orderLineDetails);
        }

        [HttpGet("{orderLineDetailId:int}", Name = "GetOrderLineDetailById")]
        [AuthorizePermission("OrderLineDetails", "View")]
        public async Task<IActionResult> GetOrderLineDetail(int orderLineDetailId)
        {
            var orderLineDetail = await _service.OrderLineDetailService.GetOrderLineDetailAsync(orderLineDetailId, trackChanges: false);
            return Ok(orderLineDetail);
        }

        [HttpGet("by-order/{orderId:guid}")]
        [AuthorizePermission("OrderLineDetails", "View")]
        public async Task<IActionResult> GetOrderLineDetailsByOrder(Guid orderId)
        {
            var orderLineDetails = await _service.OrderLineDetailService.GetOrderLineDetailsByOrderAsync(orderId, trackChanges: false);
            return Ok(orderLineDetails);
        }

        [HttpGet("by-line/{lineId:int}")]
        [AuthorizePermission("OrderLineDetails", "View")]
        public async Task<IActionResult> GetOrderLineDetailsByLine(int lineId)
        {
            var orderLineDetails = await _service.OrderLineDetailService.GetOrderLineDetailsByLineAsync(lineId, trackChanges: false);
            return Ok(orderLineDetails);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [AuthorizePermission("OrderLineDetails", "Create")]
        public async Task<IActionResult> CreateOrderLineDetail([FromBody] OrderLineDetailForCreationDto orderLineDetail)
        {
            var createdOrderLineDetail = await _service.OrderLineDetailService.CreateOrderLineDetailAsync(orderLineDetail);
            return CreatedAtRoute("GetOrderLineDetailById", new { orderLineDetailId = createdOrderLineDetail.Id }, createdOrderLineDetail);
        }

        [HttpPut("{orderLineDetailId:int}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [AuthorizePermission("OrderLineDetails", "Update")]
        public async Task<IActionResult> UpdateOrderLineDetail(int orderLineDetailId, [FromBody] OrderLineDetailForUpdateDto orderLineDetailForUpdate)
        {
            await _service.OrderLineDetailService.UpdateOrderLineDetailAsync(orderLineDetailId, orderLineDetailForUpdate, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{orderLineDetailId:int}")]
        [AuthorizePermission("OrderLineDetails", "Delete")]
        public async Task<IActionResult> DeleteOrderLineDetail(int orderLineDetailId)
        {
            await _service.OrderLineDetailService.DeleteOrderLineDetailAsync(orderLineDetailId, trackChanges: false);
            return NoContent();
        }
    }
}