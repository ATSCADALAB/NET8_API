using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickStart.Presentation.ActionFilters;
using Service.Contracts;
using Shared.DataTransferObjects.Dashboard;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QuickStart.Presentation.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    [Authorize] // Uncomment nếu bạn muốn yêu cầu xác thực
    public class DashboardController : ControllerBase
    {
        private readonly IServiceManager _service;

        public DashboardController(IServiceManager service)
        {
            _service = service;
        }

        // GET: api/dashboard/summary
        [HttpGet("summary")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            var summary = await _service.DashboardService.GetDashboardSummaryAsync();
            return Ok(summary);
        }

        // GET: api/dashboard/orders-by-line?startDate=2023-01-01&endDate=2023-12-31
        [HttpGet("orders-by-line")]
        public async Task<IActionResult> GetOrdersByLine([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var ordersByLine = await _service.DashboardService.GetOrdersByLineAsync(startDate, endDate);
            return Ok(ordersByLine);
        }

        // GET: api/dashboard/order-status-trend?startDate=2023-01-01&endDate=2023-12-31
        [HttpGet("order-status-trend")]
        public async Task<IActionResult> GetOrderStatusTrend([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var trend = await _service.DashboardService.GetOrderStatusTrendAsync(startDate, endDate);
            return Ok(trend);
        }

        // GET: api/dashboard/top-products?startDate=2023-01-01&endDate=2023-12-31
        [HttpGet("top-products")]
        public async Task<IActionResult> GetTopProducts([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var topProducts = await _service.DashboardService.GetTopProductsAsync(startDate, endDate);
            return Ok(topProducts);
        }

        // GET: api/dashboard/incomplete-orders
        [HttpGet("incomplete-orders")]
        public async Task<IActionResult> GetIncompleteOrders()
        {
            var incompleteOrders = await _service.DashboardService.GetIncompleteOrdersAsync();
            return Ok(incompleteOrders);
        }

        // GET: api/dashboard/processing-orders
        [HttpGet("processing-orders")]
        public async Task<IActionResult> GetProcessingOrders()
        {
            var processingOrders = await _service.DashboardService.GetProcessingOrdersAsync();
            return Ok(processingOrders);
        }

        // GET: api/dashboard/recent-completed-orders
        [HttpGet("recent-completed-orders")]
        public async Task<IActionResult> GetRecentCompletedOrders()
        {
            var recentOrders = await _service.DashboardService.GetRecentCompletedOrdersAsync();
            return Ok(recentOrders);
        }
    }
}