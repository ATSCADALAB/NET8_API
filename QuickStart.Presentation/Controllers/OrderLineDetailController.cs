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

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetOrderLineDetailById(Guid id)
        {
            try
            {
                var orderLineDetail = await _service.OrderLineDetailService.GetOrderLineDetailByIdAsync(id);
                if (orderLineDetail == null)
                    return NotFound($"Order line detail with ID {id} not found.");

                return Ok(orderLineDetail);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving order line detail: {ex.Message}");
            }
        }

                return Ok(orderLineDetail);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving order line detail: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<ActionResult<OrderLineDetailDto>> CreateOrderLineDetail([FromBody] OrderLineDetailForCreationDto orderLineDetailDto)
        {
            try
            {
                if (orderLineDetailDto == null)
                    return BadRequest("Order line detail data is null.");

                var createdOrderLineDetail = await _service.OrderLineDetailService.CreateOrderLineDetailAsync(orderLineDetailDto);
                return null;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating order line detail: {ex.Message}");
            }
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