using Shared.DataTransferObjects.Dashboard;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetDashboardSummaryAsync();
        Task<IEnumerable<OrdersByLineDto>> GetOrdersByLineAsync(DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<OrderStatusTrendDto>> GetOrderStatusTrendAsync(DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<TopProductDto>> GetTopProductsAsync(DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<IncompleteOrderDto>> GetIncompleteOrdersAsync();
        Task<IEnumerable<ProcessingOrderDto>> GetProcessingOrdersAsync();
        Task<IEnumerable<RecentCompletedOrderDto>> GetRecentCompletedOrdersAsync();
    }
}