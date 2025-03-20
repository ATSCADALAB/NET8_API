using AutoMapper;
using Contracts;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service
{
    internal sealed class DashboardService : IDashboardService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public DashboardService(
            IRepositoryManager repository,
            ILoggerManager logger,
            IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
        {
            try
            {
                var today = DateTime.Today;
                var orders = await _repository.Order.GetAllOrdersAsync(trackChanges: false);
                var distributors = await _repository.Distributor.GetAllDistributorsAsync(trackChanges: false);
                var areas = await _repository.Area.GetAllAreasAsync(trackChanges: false);

                var summary = new DashboardSummaryDto(
                    TotalOrdersToday: orders.Count(o => o.ExportDate.Date == today),
                    PendingOrders: orders.Count(o => o.Status == 0 || o.Status == 3), // Pending + Incomplete
                    CompletedOrdersToday: orders.Count(o => o.Status == 2 && o.ExportDate.Date == today),
                    TotalDistributors: distributors.Count(d => d.IsActive),
                    TotalAreas: areas.Count()
                );

                _logger.LogInfo("Dashboard summary retrieved successfully.");
                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving dashboard summary: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<OrdersByLineDto>> GetOrdersByLineAsync(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var orderLineDetails = await _repository.OrderLineDetail.GetAllOrderLineDetailsAsync(trackChanges: false);
                var orders = await _repository.Order.GetAllOrdersAsync(trackChanges: false);
                var lines = await _repository.Line.GetAllLinesAsync(trackChanges: false);

                var filteredOrderLines = from ol in orderLineDetails
                                         join o in orders on ol.OrderId equals o.Id
                                         where (!startDate.HasValue || o.ExportDate >= startDate) &&
                                               (!endDate.HasValue || o.ExportDate <= endDate)
                                         select new { OrderLineDetail = ol, Order = o };

                var ordersByLine = filteredOrderLines
                    .Join(lines,
                        ol => ol.OrderLineDetail.LineId,
                        l => l.Id,
                        (ol, l) => new { ol.OrderLineDetail.OrderId, LineName = l.LineName ?? "Unknown" })
                    .GroupBy(x => x.LineName)
                    .Select(g => new OrdersByLineDto(
                        LineName: g.Key,
                        TotalOrders: g.Select(x => x.OrderId).Distinct().Count() // Đếm số đơn hàng duy nhất
                    ));

                _logger.LogInfo("Orders by line retrieved successfully.");
                return ordersByLine;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving orders by line: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<OrderStatusTrendDto>> GetOrderStatusTrendAsync(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var orders = await _repository.Order.GetAllOrdersAsync(trackChanges: false);
                var filteredOrders = orders.Where(o =>
                    (!startDate.HasValue || o.ExportDate >= startDate) &&
                    (!endDate.HasValue || o.ExportDate <= endDate));

                var trend = filteredOrders
                    .GroupBy(o => o.ExportDate.Date)
                    .Select(g => new OrderStatusTrendDto(
                        Date: g.Key.ToString("yyyy-MM-dd"),
                        Pending: g.Count(o => o.Status == 0),
                        Processing: g.Count(o => o.Status == 1),
                        Incomplete: g.Count(o => o.Status == 3),
                        Completed: g.Count(o => o.Status == 2)
                    ))
                    .OrderBy(t => t.Date);

                _logger.LogInfo("Order status trend retrieved successfully.");
                return trend;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving order status trend: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TopProductDto>> GetTopProductsAsync(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var orderDetails = await _repository.OrderDetail.GetAllOrderDetailsAsync(trackChanges: false);
                var orders = await _repository.Order.GetAllOrdersAsync(trackChanges: false);
                var sensorRecords = await _repository.SensorRecord.GetAllSensorRecordsAsync(trackChanges: false);
                var productInfos = await _repository.ProductInformation.GetAllProductInformationsAsync(trackChanges: false);

                var filteredOrderDetails = from od in orderDetails
                                           join o in orders on od.OrderId equals o.Id
                                           where (!startDate.HasValue || o.ExportDate >= startDate) &&
                                                 (!endDate.HasValue || o.ExportDate <= endDate)
                                           select od;

                var topProducts = filteredOrderDetails
                    .Join(productInfos,
                        od => od.ProductInformationId,
                        pi => pi.Id,
                        (od, pi) => new { OrderDetail = od, ProductName = pi.ProductName })
                    .GroupBy(x => x.ProductName)
                    .Select(g => new TopProductDto(
                        ProductName: g.Key,
                        TotalUnits: g.Sum(x => sensorRecords
                            .Where(sr => sr.OrderDetailId == x.OrderDetail.Id)
                            .Sum(sr => sr.SensorUnits) > 0
                            ? sensorRecords.Where(sr => sr.OrderDetailId == x.OrderDetail.Id).Sum(sr => sr.SensorUnits)
                            : x.OrderDetail.RequestedUnits)
                    ))
                    .OrderByDescending(p => p.TotalUnits)
                    .Take(5);

                _logger.LogInfo("Top products retrieved successfully.");
                return topProducts;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving top products: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<IncompleteOrderDto>> GetIncompleteOrdersAsync()
        {
            try
            {
                var orders = await _repository.Order.GetAllOrdersAsync(trackChanges: false);
                var orderDetails = await _repository.OrderDetail.GetAllOrderDetailsAsync(trackChanges: false);
                var sensorRecords = await _repository.SensorRecord.GetAllSensorRecordsAsync(trackChanges: false);
                var productInfos = await _repository.ProductInformation.GetAllProductInformationsAsync(trackChanges: false);

                var incompleteOrders = from o in orders
                                       where o.Status != 2 // Lọc các đơn chưa hoàn thành
                                       join od in orderDetails on o.Id equals od.OrderId
                                       join pi in productInfos on od.ProductInformationId equals pi.Id
                                       group new { o, od, pi } by o into g
                                       select new IncompleteOrderDto(
                                           Date: g.Key.ExportDate.ToString("yyyy-MM-dd"),
                                           OrderNumber: g.Key.OrderCode,
                                           VehicleNumber: g.Key.VehicleNumber ?? "N/A",
                                           ProductName: g.First().pi.ProductName,
                                           RequestedUnits: g.Sum(x => x.od.RequestedUnits),
                                           ActualUnits: sensorRecords
                                               .Where(sr => sr.OrderId == g.Key.Id)
                                               .Sum(sr => sr.SensorUnits),
                                           CompletionPercentage: g.Sum(x => x.od.RequestedUnits) > 0
                                               ? (decimal)sensorRecords.Where(sr => sr.OrderId == g.Key.Id).Sum(sr => sr.SensorUnits) / g.Sum(x => x.od.RequestedUnits) * 100
                                               : 0
                                       );

                _logger.LogInfo("Incomplete orders retrieved successfully.");
                return incompleteOrders;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving incomplete orders: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<ProcessingOrderDto>> GetProcessingOrdersAsync()
        {
            try
            {
                var orders = await _repository.Order.GetAllOrdersAsync(trackChanges: false);
                var orderDetails = await _repository.OrderDetail.GetAllOrderDetailsAsync(trackChanges: false);
                var orderLineDetails = await _repository.OrderLineDetail.GetAllOrderLineDetailsAsync(trackChanges: false);
                var lines = await _repository.Line.GetAllLinesAsync(trackChanges: false);
                var sensorRecords = await _repository.SensorRecord.GetAllSensorRecordsAsync(trackChanges: false);

                var processingOrders = from o in orders
                                       where o.Status == 1 // Chỉ lấy đơn đang xử lý
                                       join od in orderDetails on o.Id equals od.OrderId
                                       join ol in orderLineDetails on o.Id equals ol.OrderId into ols
                                       from ol in ols.DefaultIfEmpty()
                                       join l in lines on ol?.LineId equals l?.Id into ls
                                       from l in ls.DefaultIfEmpty()
                                       group new { o, od, LineName = l?.LineName } by o into g
                                       select new ProcessingOrderDto(
                                           Date: g.Key.ExportDate.ToString("yyyy-MM-dd"),
                                           OrderNumber: g.Key.OrderCode,
                                           VehicleNumber: g.Key.VehicleNumber ?? "N/A",
                                           LineName: g.First().LineName ?? "Unknown",
                                           TotalUnits: g.Sum(x => sensorRecords
                                               .Where(sr => sr.OrderDetailId == x.od.Id)
                                               .Sum(sr => sr.SensorUnits) > 0
                                               ? sensorRecords.Where(sr => sr.OrderDetailId == x.od.Id).Sum(sr => sr.SensorUnits)
                                               : x.od.RequestedUnits),
                                           Status: "Processing"
                                       );

                _logger.LogInfo("Processing orders retrieved successfully.");
                return processingOrders;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving processing orders: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<RecentCompletedOrderDto>> GetRecentCompletedOrdersAsync()
        {
            try
            {
                var orders = await _repository.Order.GetAllOrdersAsync(trackChanges: false);
                var orderDetails = await _repository.OrderDetail.GetAllOrderDetailsAsync(trackChanges: false);
                var distributors = await _repository.Distributor.GetAllDistributorsAsync(trackChanges: false);
                var productInfos = await _repository.ProductInformation.GetAllProductInformationsAsync(trackChanges: false);
                var sensorRecords = await _repository.SensorRecord.GetAllSensorRecordsAsync(trackChanges: false);

                var recentOrders = (from o in orders
                                    where o.Status == 2 // Chỉ lấy đơn đã hoàn thành
                                    join od in orderDetails on o.Id equals od.OrderId
                                    join d in distributors on o.DistributorId equals d.Id
                                    join pi in productInfos on od.ProductInformationId equals pi.Id
                                    orderby o.ExportDate descending
                                    select new { o, od, d, pi })
                                   .Take(5)
                                   .GroupBy(x => x.o)
                                   .Select(g => new RecentCompletedOrderDto(
                                       CompletedDate: g.Key.ExportDate.ToString("yyyy-MM-dd"),
                                       OrderNumber: g.Key.OrderCode,
                                       DistributorName: g.First().d.DistributorName ?? "N/A",
                                       ProductName: g.First().pi.ProductName,
                                       TotalUnits: g.Sum(x => sensorRecords
                                           .Where(sr => sr.OrderDetailId == x.od.Id)
                                           .Sum(sr => sr.SensorUnits) > 0
                                           ? sensorRecords.Where(sr => sr.OrderDetailId == x.od.Id).Sum(sr => sr.SensorUnits)
                                           : x.od.RequestedUnits)
                                   ));

                _logger.LogInfo("Recent completed orders retrieved successfully.");
                return recentOrders;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving recent completed orders: {ex.Message}");
                throw;
            }
        }
    }
}