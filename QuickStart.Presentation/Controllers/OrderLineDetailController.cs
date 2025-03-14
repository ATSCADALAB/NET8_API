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

        
    }
}