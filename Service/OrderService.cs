using AutoMapper;
using ClosedXML.Excel;
using Contracts;
using Entities.Exceptions.Order;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Service.Contracts;
using Shared.DataTransferObjects.Order;
using Shared.DataTransferObjects.OrderDetail;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    internal sealed class OrderService : IOrderService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly string _connectionString;
        public OrderService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, IConfiguration configuration)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _connectionString = configuration.GetConnectionString("sqlConnection");
        }
        public async Task<ImportResult> ImportOrdersFromExcelAsync(IFormFile file)
        {
            var errors = new List<string>();
            var skippedRows = new List<string>();
            var successCount = 0;

            try
            {
                // Lấy tất cả distributors và products một lần
                var allDistributors = await _repository.Distributor.GetAllDistributorsAsync(false);
                var allProducts = await _repository.ProductInformation.GetAllProductInformationsAsync(false);

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheet(1);
                        if (worksheet == null)
                            throw new Exception("Excel file has no valid worksheet.");

                        var rowCount = worksheet.RowsUsed().Count();
                        if (rowCount < 2)
                            throw new Exception("Excel file is empty or has no data rows.");

                        using (var connection = new MySqlConnection(_connectionString))
                        {
                            await connection.OpenAsync();

                            // Xử lý từng dòng Excel
                            for (int row = 4; row <= rowCount + 2; row++)
                            {
                                try
                                {
                                    using (var command = connection.CreateCommand())
                                    {
                                        command.CommandText = "sp_ImportOrder";
                                        command.CommandType = System.Data.CommandType.StoredProcedure;

                                        var orderCode = worksheet.Cell(row, 2).GetValue<string>()?.Trim();
                                        if (string.IsNullOrWhiteSpace(orderCode))
                                            throw new Exception("OrderCode is missing.");

                                        var exportDate = worksheet.Cell(row, 1).GetValue<DateTime>();
                                        var drivernumber = worksheet.Cell(row, 3).GetValue<int>();
                                        var vehicleNumber = worksheet.Cell(row, 9).GetValue<string>()?.Trim();
                                        var driverName = worksheet.Cell(row, 12).GetValue<string>()?.Trim().Replace("_x000D_", "").Trim();
                                        var driverPhoneNumber = worksheet.Cell(row, 13).GetValue<string>()?.Trim();
                                        var distributorName = worksheet.Cell(row, 14).GetValue<string>()?.Trim();
                                        var productCode = worksheet.Cell(row, 4).GetValue<string>()?.Trim();
                                        var requestedWeight = worksheet.Cell(row, 6).GetValue<decimal>();
                                        var requestedUnits = worksheet.Cell(row, 7).GetValue<int>();

                                        DateTime? manufactureDate = null;
                                        var manufactureDateCell = worksheet.Cell(row, 8);
                                        if (!manufactureDateCell.IsEmpty())
                                        {
                                            try
                                            {
                                                manufactureDate = manufactureDateCell.GetValue<DateTime>();
                                                if (manufactureDate == DateTime.MinValue)
                                                    manufactureDate = null;
                                            }
                                            catch
                                            {
                                                manufactureDate = null;
                                            }
                                        }

                                        // Validate dữ liệu
                                        if (exportDate == default || string.IsNullOrWhiteSpace(vehicleNumber) ||
                                            string.IsNullOrWhiteSpace(driverName) || string.IsNullOrWhiteSpace(driverPhoneNumber) ||
                                            string.IsNullOrWhiteSpace(distributorName))
                                            throw new Exception("Missing required Order fields.");

                                        // Tìm distributor và product từ cache
                                        var distributor = allDistributors.FirstOrDefault(d => 
                                            d.DistributorName.Equals(distributorName, StringComparison.OrdinalIgnoreCase));
                                        if (distributor == null)
                                            throw new Exception($"Distributor '{distributorName}' not found.");

                                        var product = allProducts.FirstOrDefault(p => 
                                            p.ProductCode.Equals(productCode, StringComparison.OrdinalIgnoreCase));
                                        if (product == null)
                                            throw new Exception($"Product '{productCode}' not found.");

                                        // Thêm parameters
                                        command.Parameters.Add(new MySqlParameter("@p_order_code", orderCode));
                                        command.Parameters.Add(new MySqlParameter("@p_export_date", exportDate));
                                        command.Parameters.Add(new MySqlParameter("@p_driver_number", drivernumber));
                                        command.Parameters.Add(new MySqlParameter("@p_vehicle_number", vehicleNumber));
                                        command.Parameters.Add(new MySqlParameter("@p_driver_name", driverName));
                                        command.Parameters.Add(new MySqlParameter("@p_driver_phone_number", driverPhoneNumber));
                                        command.Parameters.Add(new MySqlParameter("@p_distributor_id", distributor.Id));
                                        command.Parameters.Add(new MySqlParameter("@p_product_information_id", product.Id));
                                        command.Parameters.Add(new MySqlParameter("@p_requested_units", requestedUnits));
                                        command.Parameters.Add(new MySqlParameter("@p_requested_weight", requestedWeight));
                                        command.Parameters.Add(new MySqlParameter("@p_manufacture_date", 
                                            (object)manufactureDate ?? DBNull.Value));

                                        using (var reader = await command.ExecuteReaderAsync())
                                        {
                                            if (await reader.ReadAsync())
                                            {
                                                var isDuplicate = reader.GetInt32(0);
                                                if (isDuplicate == 1)
                                                {
                                                    skippedRows.Add($"Row {row}: {reader.GetString(1)}");
                                                }
                                                else
                                                {
                                                    successCount++;
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    errors.Add($"Row {row}: {ex.Message}");
                                }
                            }
                        }
                    }
                }

                return new ImportResult
                {
                    SuccessCount = successCount,
                    SkippedCount = skippedRows.Count,
                    SkippedRows = skippedRows,
                    Errors = errors
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error importing orders: {ex.Message}");
                throw new Exception($"Error importing orders: {ex.Message}");
            }
        }
        public async Task<IEnumerable<OrderWithDetailsDto>> GetOrdersByFilterAsync(
            DateTime startDate,
            DateTime endDate,
            int? distributorId,
            int? areaId,
            int? productInformationId,
            int? status,
            bool trackChanges)
        {
            _logger.LogInfo($"Fetching Orders from {startDate} to {endDate}, DistributorId: {distributorId}, AreaId: {areaId}, ProductInformationId: {productInformationId}, Status: {status}");

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "GetOrdersByFilter";
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        // Thêm các tham số
                        command.Parameters.Add(new MySqlParameter("@p_start_date", startDate));
                        command.Parameters.Add(new MySqlParameter("@p_end_date", endDate));
                        command.Parameters.Add(new MySqlParameter("@p_distributor_id", distributorId ?? (object)DBNull.Value));
                        command.Parameters.Add(new MySqlParameter("@p_area_id", areaId ?? (object)DBNull.Value));
                        command.Parameters.Add(new MySqlParameter("@p_product_information_id", productInformationId ?? (object)DBNull.Value));
                        command.Parameters.Add(new MySqlParameter("@p_status", status ?? (object)DBNull.Value));

                        var orders = new List<OrderWithDetailsDto>();
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                orders.Add(new OrderWithDetailsDto
                                {
                                    Id = reader.GetGuid("Id"),
                                    OrderCode = reader.GetString("OrderCode"),
                                    ExportDate = reader.GetDateTime("ExportDate"),
                                    VehicleNumber = reader.GetString("VehicleNumber"),
                                    DriverNumber = reader.GetInt32("DriverNumber"),
                                    DriverName = reader.GetString("DriverName"),
                                    DriverPhoneNumber = reader.GetString("DriverPhoneNumber"),
                                    Status = reader.GetInt32("Status"),
                                    DistributorId = reader.GetInt32("DistributorId"),
                                    DistributorName = reader.GetString("DistributorName"),
                                    Area = reader.GetString("AreaName"),
                                    CreatedAt = reader.GetDateTime("CreatedAt"),
                                    UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime("UpdatedAt"),
                                    OrderDetail = reader.IsDBNull(reader.GetOrdinal("OrderDetailId")) ? null : new OrderDetailWithProductDto
                                    {
                                        Id = reader.GetInt32("OrderDetailId"),
                                        OrderId = reader.GetGuid("OrderId"),
                                        ProductInformationId = reader.GetInt32("ProductInformationId"),
                                        ProductCode = reader.GetString("ProductCode"),
                                        ProductName = reader.GetString("ProductName"),
                                        RequestedUnits = reader.GetInt32("RequestedUnits"),
                                        RequestedWeight = reader.GetDecimal("RequestedWeight"),
                                        ManufactureDate = reader.IsDBNull(reader.GetOrdinal("ManufactureDate")) ? null : reader.GetDateTime("ManufactureDate"),
                                        DefectiveUnits = reader.GetInt32("DefectiveUnits"),
                                        DefectiveWeight = reader.GetDecimal("DefectiveWeight"),
                                        ReplacedUnits = reader.GetInt32("ReplacedUnits"),
                                        ReplacedWeight = reader.GetDecimal("ReplacedWeight"),
                                        CreatedAt = reader.GetDateTime("OrderDetailCreatedAt")
                                    }
                                });
                            }
                        }

                        if (!orders.Any())
                        {
                            _logger.LogInfo("No orders found for the given filter.");
                            return new List<OrderWithDetailsDto>(); // Trả về danh sách rỗng
                        }

                        return orders;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching Orders: {ex.Message}");
                throw;
            }
        }
        
        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync(bool trackChanges)
        {
            var orders = await _repository.Order.GetAllOrdersAsync(trackChanges);
            var ordersDto = _mapper.Map<IEnumerable<OrderDto>>(orders);
            return ordersDto;
        }

        public async Task<OrderDto> GetOrderAsync(Guid orderId, bool trackChanges)
        {
            var order = await GetOrderAndCheckIfItExists(orderId, trackChanges);
            var orderDto = _mapper.Map<OrderDto>(order);
            return orderDto;
        }

        public async Task<OrderDto> GetOrderByCodeAsync(string orderCode, bool trackChanges)
        {
            var order = await _repository.Order.GetOrderByCodeAsync(orderCode, trackChanges);
            if (order is null)
                throw new OrderNotFoundException(Guid.Empty); // Guid không quan trọng vì dùng Code

            var orderDto = _mapper.Map<OrderDto>(order);
            return orderDto;
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByDistributorAsync(int distributorId, bool trackChanges)
        {
            var orders = await _repository.Order.GetOrdersByDistributorAsync(distributorId, trackChanges);
            var ordersDto = _mapper.Map<IEnumerable<OrderDto>>(orders);
            return ordersDto;
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByExportDateAsync(DateTime exportDate, bool trackChanges)
        {
            var orders = await _repository.Order.GetOrdersByExportDateAsync(exportDate, trackChanges);
            var ordersDto = _mapper.Map<IEnumerable<OrderDto>>(orders);
            return ordersDto;
        }

        public async Task<OrderDto> CreateOrderAsync(OrderForCreationDto order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order), "OrderForCreationDto cannot be null.");

            var orderEntity = _mapper.Map<Order>(order);
            _repository.Order.CreateOrder(orderEntity);
            await _repository.SaveAsync();

            var orderToReturn = _mapper.Map<OrderDto>(orderEntity);
            return orderToReturn;
        }

        public async Task UpdateOrderAsync(Guid orderId, OrderForUpdateDto orderForUpdate, bool trackChanges)
        {
            var order = await GetOrderAndCheckIfItExists(orderId, trackChanges);
            order.UpdatedAt = DateTime.Now;
            _mapper.Map(orderForUpdate, order);
            await _repository.SaveAsync();
        }

        public async Task DeleteOrderAsync(Guid orderId, bool trackChanges)
        {
            var order = await GetOrderAndCheckIfItExists(orderId, trackChanges);
            _repository.Order.DeleteOrder(order);
            await _repository.SaveAsync();
        }

        private async Task<Order> GetOrderAndCheckIfItExists(Guid id, bool trackChanges)
        {
            var order = await _repository.Order.GetOrderByIdAsync(id, trackChanges);
            if (order is null)
                throw new OrderNotFoundException(id);
            return order;
        }
    }
}