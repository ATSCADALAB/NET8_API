using AutoMapper;
using Contracts;
using Entities.Exceptions.OrderLineDetail;
using Entities.Models;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Service.Contracts;
using Shared.DataTransferObjects.OrderLineDetail;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    internal sealed class OrderLineDetailService : IOrderLineDetailService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly string _connectionString;

        public OrderLineDetailService(
            IRepositoryManager repository,
            ILoggerManager logger,
            IMapper mapper,
            IConfiguration configuration)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _connectionString = configuration.GetConnectionString("sqlConnection");
        }

        public async Task<IEnumerable<OrderLineDetailDto>> GetAllOrderLineDetailsAsync(bool trackChanges)
        {
            var orderLineDetails = await _repository.OrderLineDetail.GetAllOrderLineDetailsAsync(trackChanges);
            var orderLineDetailsDto = _mapper.Map<IEnumerable<OrderLineDetailDto>>(orderLineDetails);
            return orderLineDetailsDto;
        }

        public async Task<OrderLineDetailDto> GetOrderLineDetailAsync(int orderLineDetailId, bool trackChanges)
        {
            var orderLineDetail = await GetOrderLineDetailAndCheckIfItExists(orderLineDetailId, trackChanges);
            var orderLineDetailDto = _mapper.Map<OrderLineDetailDto>(orderLineDetail);
            return orderLineDetailDto;
        }

        public async Task<IEnumerable<OrderLineDetailDto>> GetOrderLineDetailsByOrderAsync(Guid orderId, bool trackChanges)
        {
            var orderLineDetails = await _repository.OrderLineDetail.GetOrderLineDetailsByOrderIdAsync(orderId, trackChanges);
            var orderLineDetailsDto = _mapper.Map<IEnumerable<OrderLineDetailDto>>(orderLineDetails);
            return orderLineDetailsDto;
        }

        public async Task<IEnumerable<OrderLineDetailDto>> GetOrderLineDetailsByLineAsync(int lineId, bool trackChanges)
        {
            var orderLineDetails = await _repository.OrderLineDetail.GetOrderLineDetailsByLineIdAsync(lineId, trackChanges);
            var orderLineDetailsDto = _mapper.Map<IEnumerable<OrderLineDetailDto>>(orderLineDetails);
            return orderLineDetailsDto;
        }

        public async Task<OrderLineDetailDto> CreateOrderLineDetailAsync(OrderLineDetailForCreationDto orderLineDetail)
        {
            if (orderLineDetail == null)
                throw new ArgumentNullException(nameof(orderLineDetail), "OrderLineDetailForCreationDto cannot be null.");

            var orderLineDetailEntity = _mapper.Map<OrderLineDetail>(orderLineDetail);
            orderLineDetailEntity.EndTime = null;
            _repository.OrderLineDetail.CreateOrderLineDetail(orderLineDetailEntity);
            await _repository.SaveAsync();

            var orderLineDetailToReturn = _mapper.Map<OrderLineDetailDto>(orderLineDetailEntity);
            return orderLineDetailToReturn;
        }

        public async Task UpdateOrderLineDetailAsync(int orderLineDetailId, OrderLineDetailForUpdateDto orderLineDetailForUpdate, bool trackChanges)
        {
            var orderLineDetail = await GetOrderLineDetailAndCheckIfItExists(orderLineDetailId, trackChanges);
            _mapper.Map(orderLineDetailForUpdate, orderLineDetail);
            await _repository.SaveAsync();
        }

        public async Task DeleteOrderLineDetailAsync(int orderLineDetailId, bool trackChanges)
        {
            var orderLineDetail = await GetOrderLineDetailAndCheckIfItExists(orderLineDetailId, trackChanges);
            _repository.OrderLineDetail.DeleteOrderLineDetail(orderLineDetail);
            await _repository.SaveAsync();
        }

        // Thêm phương thức mới để gọi stored procedure
        public async Task<IEnumerable<RunningOrderDto>> GetRunningOrdersByLineAsync(int lineId)
        {
            _logger.LogInfo($"Fetching running orders for LineId: {lineId}");

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "GetRunningOrdersByLineNoCursor";
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        // Thêm tham số lineId
                        command.Parameters.Add(new MySqlParameter("@lineId", lineId));

                        var runningOrders = new List<RunningOrderDto>();
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            
                            while (await reader.ReadAsync())
                            {
                                runningOrders.Add(new RunningOrderDto
                                {
                                    LineId = reader.GetInt32("LineId"),
                                    LineNumber = reader.GetInt32("LineNumber"),
                                    LineName = reader.GetString("LineName"),
                                    OrderId = reader.GetGuid("OrderId"),
                                    OrderCode = reader.GetString("OrderCode"),
                                    DistributorId = reader.GetInt32("DistributorId"),
                                    DistributorName = reader.GetString("DistributorName"),
                                    ProductInformationId = reader.GetInt32("ProductInformationId"),
                                    ProductName = reader.GetString("ProductName"),
                                    RequestedUnits = reader.GetInt32("RequestedUnits"),
                                    VehicleNumber = reader.IsDBNull(reader.GetOrdinal("VehicleNumber"))
                                        ? null
                                        : reader.GetString("VehicleNumber"),
                                    ExportDate = reader.GetDateTime("ExportDate"),
                                    RecordTime = reader.GetDateTime("RecordTime")
                                });
                            }
                        }

                        if (!runningOrders.Any())
                        {
                            _logger.LogInfo($"No running orders found for LineId: {lineId}.");
                            return new List<RunningOrderDto>(); // Trả về danh sách rỗng
                        }

                        return runningOrders;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching running orders for LineId: {lineId}: {ex.Message}");
                throw;
            }
        }

        private async Task<OrderLineDetail> GetOrderLineDetailAndCheckIfItExists(int id, bool trackChanges)
        {
            var orderLineDetail = await _repository.OrderLineDetail.GetOrderLineDetailByIdAsync(id, trackChanges);
            if (orderLineDetail is null)
                throw new OrderLineDetailNotFoundException(id);
            return orderLineDetail;
        }
    }
}