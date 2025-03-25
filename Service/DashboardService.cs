using AutoMapper;
using Contracts;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Service.Contracts;
using Shared.DataTransferObjects.Dashboard;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Service
{
    internal sealed class DashboardService : IDashboardService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly string _connectionString;

        public DashboardService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, IConfiguration configuration)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _connectionString = configuration.GetConnectionString("sqlConnection");

        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "GetDashboardSummary";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@p_today", DateTime.Today);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var summary = new DashboardSummaryDto(
                                    reader.GetInt32("TotalOrdersToday"),
                                    reader.GetInt32("PendingOrders"),
                                    reader.GetInt32("CompletedOrdersToday"),
                                    reader.GetInt32("TotalDistributors"),
                                    reader.GetInt32("TotalAreas")
                                );
                                _logger.LogInfo("Dashboard summary retrieved successfully.");
                                return summary;
                            }
                        }
                    }
                }
                throw new Exception("No data returned from GetDashboardSummary");
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
                var result = new List<OrdersByLineDto>();
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "GetOrdersByLine";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@p_start_date", startDate.HasValue ? startDate : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@p_end_date", endDate.HasValue ? endDate : (object)DBNull.Value);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                result.Add(new OrdersByLineDto(
                                    reader.GetString("LineName"),
                                    reader.GetInt32("TotalOrders")
                                ));
                            }
                        }
                    }
                }
                _logger.LogInfo("Orders by line retrieved successfully.");
                return result;
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
                var result = new List<OrderStatusTrendDto>();
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "GetOrderStatusTrend";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@p_start_date", startDate.HasValue ? startDate : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@p_end_date", endDate.HasValue ? endDate : (object)DBNull.Value);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                result.Add(new OrderStatusTrendDto(
                                    reader.GetDateTime("Date").ToString(),
                                    reader.GetInt32("Pending"),
                                    reader.GetInt32("Processing"),
                                    reader.GetInt32("Incomplete"),
                                    reader.GetInt32("Completed")
                                ));
                            }
                        }
                    }
                }
                _logger.LogInfo("Order status trend retrieved successfully.");
                return result;
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
                var result = new List<TopProductDto>();
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "GetTopProducts";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@p_start_date", startDate.HasValue ? startDate : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@p_end_date", endDate.HasValue ? endDate : (object)DBNull.Value);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                result.Add(new TopProductDto(
                                    reader.GetString("ProductName"),
                                    reader.GetInt32("TotalUnits")
                                ));
                            }
                        }
                    }
                }
                _logger.LogInfo("Top products retrieved successfully.");
                return result;
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
                var result = new List<IncompleteOrderDto>();
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "GetIncompleteOrders";
                        command.CommandType = CommandType.StoredProcedure;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                result.Add(new IncompleteOrderDto(
                                    reader.GetDateTime("Date").ToString(),
                                    reader.GetString("OrderNumber"),
                                    reader.GetString("VehicleNumber"),
                                    reader.GetString("ProductName"),
                                    reader.GetInt32("RequestedUnits"),
                                    reader.GetInt32("ActualUnits"),
                                    reader.GetDecimal("CompletionPercentage")
                                ));
                            }
                        }
                    }
                }
                _logger.LogInfo("Incomplete orders retrieved successfully.");
                return result;
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
                var result = new List<ProcessingOrderDto>();
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "GetProcessingOrders";
                        command.CommandType = CommandType.StoredProcedure;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                result.Add(new ProcessingOrderDto(
                                    reader.GetDateTime("Date").ToString(),
                                    reader.GetString("OrderNumber"),
                                    reader.GetString("VehicleNumber"),
                                    reader.GetString("LineName"),
                                    reader.GetInt32("TotalUnits"),
                                    reader.GetString("Status")
                                ));
                            }
                        }
                    }
                }
                _logger.LogInfo("Processing orders retrieved successfully.");
                return result;
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
                var result = new List<RecentCompletedOrderDto>();
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "GetRecentCompletedOrders";
                        command.CommandType = CommandType.StoredProcedure;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                result.Add(new RecentCompletedOrderDto(
                                    reader.GetDateTime("CompletedDate").ToString(),
                                    reader.GetString("OrderNumber"),
                                    reader.GetString("DistributorName"),
                                    reader.GetString("ProductName"),
                                    reader.GetInt32("TotalUnits")
                                ));
                            }
                        }
                    }
                }
                _logger.LogInfo("Recent completed orders retrieved successfully.");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving recent completed orders: {ex.Message}");
                throw;
            }
        }
    }
}