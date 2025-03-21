using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickStart.Presentation.ActionFilters;
using Service.Contracts;
using Shared.DataTransferObjects.Order;
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
        //[AuthorizePermission("OrderLineDetails", "View")]
        public async Task<IActionResult> GetAllOrderLineDetails()
        {
            var orderLineDetails = await _service.OrderLineDetailService.GetAllOrderLineDetailsAsync(trackChanges: false);
            return Ok(orderLineDetails);
        }
        [HttpGet("{orderId:guid}", Name = "GetOrderLineDetailsByOrderId")]
        //[AuthorizePermission("OrderLineDetails", "View")]
        public async Task<IActionResult> GetOrderLineDetailByOrderId(Guid orderId)
        {
            var orderLineDetails = await _service.OrderLineDetailService.GetOrderLineDetailsByOrderAsync(orderId, trackChanges: false);
            return Ok(orderLineDetails);
        }
        [HttpGet("{orderLineDetailId:int}", Name = "GetOrderLineDetailById")]
        //[AuthorizePermission("Orders", "View")]
        public async Task<IActionResult> GetOrder(int orderLineDetailId)
        {
            var orderLineDetail = await _service.OrderLineDetailService.GetOrderLineDetailAsync(orderLineDetailId, trackChanges: false);
            return Ok(orderLineDetail);
        }
        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [AuthorizePermission("OrderLineDetails", "Create")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderLineDetailForCreationDto orderLineDetail)
        {
            var createdOrderLineDetail = await _service.OrderLineDetailService.CreateOrderLineDetailAsync(orderLineDetail);
            return CreatedAtRoute("GetOrderLineDetailById", new { orderLineDetailId = createdOrderLineDetail.Id }, createdOrderLineDetail);
        }
    }
}