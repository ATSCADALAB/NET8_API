using AutoMapper;
using Contracts;
using Entities.Exceptions.Product;
using Entities.Models;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Service.Contracts;
using Shared.DataTransferObjects.Product;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Service
{
    internal sealed class ProductService : IProductService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly string _connectionString;
        public ProductService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, IConfiguration configuration)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _connectionString = configuration.GetConnectionString("sqlConnection");
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync(bool trackChanges)
        {
            var products = await _repository.Product.GetAllProductsAsync(trackChanges);
            var productsDto = _mapper.Map<IEnumerable<ProductDto>>(products);
            return productsDto;
        }

        public async Task<ProductDto> GetProductAsync(int productId, bool trackChanges)
        {
            var product = await GetProductAndCheckIfItExists(productId, trackChanges);
            var productDto = _mapper.Map<ProductDto>(product);
            return productDto;
        }

        public async Task<CheckDto> GetProductByTagIDAsync(string tagId, bool trackChanges)
        {
            _logger.LogInfo($"Fetching product with TagID: {tagId}");

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "GetProductByTagID"; // Tên stored procedure
                        command.CommandType = CommandType.StoredProcedure;

                        // Thêm tham số TagID
                        command.Parameters.Add(new MySqlParameter("@p_tag_id", tagId));

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new CheckDto
                                {
                                    TagID = reader.GetString("TagID"),
                                    ProductInformation = new CheckProductInformationDto
                                    {
                                        ProductCode = reader.GetString("ProductCode"),
                                        ProductName = reader.GetString("ProductName")
                                    },
                                    ProductDate = reader.GetDateTime("ManufactureDate"),
                                    ShipmentDate = reader.GetDateTime("ShipmentDate"),
                                    Distributor = new CheckDistributorDto
                                    {
                                        DistributorName = reader.GetString("DistributorName"),
                                        Area = reader.GetString("AreaName")
                                    },
                                    Delivery = reader.IsDBNull(reader.GetOrdinal("Delivery"))
                                        ? "N/A"
                                        : reader.GetString("Delivery")
                                };
                            }
                            else
                            {
                                _logger.LogInfo($"No product found with TagID: {tagId}");
                                return null; // Hoặc throw exception tùy yêu cầu
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching product with TagID {tagId}: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByDistributorAsync(int distributorId, bool trackChanges)
        {
            var products = await _repository.Product.GetProductsByDistributorIdAsync(distributorId, trackChanges);
            var productsDto = _mapper.Map<IEnumerable<ProductDto>>(products);
            return productsDto;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByOrderDetailAsync(int orderDetailId, bool trackChanges)
        {
            var products = await _repository.Product.GetProductsByOrderDetailIdAsync(orderDetailId, trackChanges);
            var productsDto = _mapper.Map<IEnumerable<ProductDto>>(products);
            return productsDto;
        }

        public async Task<ProductDto> CreateProductAsync(ProductForCreationDto product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product), "ProductForCreationDto cannot be null.");

            var productEntity = _mapper.Map<Product>(product);
            _repository.Product.CreateProduct(productEntity);
            await _repository.SaveAsync();

            var productToReturn = _mapper.Map<ProductDto>(productEntity);
            return productToReturn;
        }

        public async Task UpdateProductAsync(int productId, ProductForUpdateDto productForUpdate, bool trackChanges)
        {
            var product = await GetProductAndCheckIfItExists(productId, trackChanges);
            _mapper.Map(productForUpdate, product);
            await _repository.SaveAsync();
        }

        public async Task DeleteProductAsync(int productId, bool trackChanges)
        {
            var product = await GetProductAndCheckIfItExists(productId, trackChanges);
            _repository.Product.DeleteProduct(product);
            await _repository.SaveAsync();
        }

        private async Task<Product> GetProductAndCheckIfItExists(int id, bool trackChanges)
        {
            var product = await _repository.Product.GetProductByIdAsync(id, trackChanges);
            if (product is null)
                throw new ProductNotFoundException(id);
            return product;
        }
    }
}