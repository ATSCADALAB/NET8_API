using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.DataTransferObjects.OrderLineDetail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace QuickStart.Presentation.Controllers
{
    [Route("api/OrderLineDetails")]
    [ApiController]
    public class OrderLineDetailController : ControllerBase
    {
        private readonly IServiceManager _service;

        public OrderLineDetailController(IServiceManager service) => _service = service;
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderLineDetailDto>>> GetAllOrderLineDetails()
        {
            try
            {
                var orderLineDetails = await _service.OrderLineDetailService.GetAllOrderLineDetailsAsync();
                if (orderLineDetails == null || !orderLineDetails.Any())
                    return NotFound("No order line details found.");

                return Ok(orderLineDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving order line details: {ex.Message}");
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetOrderLineDetailByOrderId(Guid id)
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
        [HttpGet("byLine/{line:int}")]
        public async Task<IActionResult> GetOrderLineDetailByLine(int id)
        {
            try
            {
                var orderLineDetail = await _service.OrderLineDetailService.GetOrderLineDetailByLineIDAsync(id);
                if (orderLineDetail == null)
                    return NotFound($"Order line detail with ID {id} not found.");

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
                await _service.OrderService.UpdateStatusAsync(orderLineDetailDto.OrderId, 1);
                return null;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating order line detail: {ex.Message}");
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<OrderLineDetailDto>> UpdateOrderLineDetail(Guid id, [FromBody] OrderLineDetailForUpdateDto orderLineDetailDto)
        {
            try
            {
                if (orderLineDetailDto == null)
                    return BadRequest("Order line detail update data is null.");

                var updatedOrderLineDetail = await _service.OrderLineDetailService.UpdateOrderLineDetailAsync(id, orderLineDetailDto);
                if (updatedOrderLineDetail == null)
                    return NotFound($"Order line detail with ID {id} not found.");

                return Ok(updatedOrderLineDetail);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating order line detail: {ex.Message}");
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteOrderLineDetail(Guid id)
        {
            try
            {
                await _service.OrderLineDetailService.DeleteOrderLineDetailAsync(id, trackChanges: false);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting order line detail: {ex.Message}");
            }
        }
    }
}