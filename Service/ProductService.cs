using AutoMapper;
using ClosedXML.Excel;
using Contracts;
using Entities.Exceptions.Product;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Repository;
using Service.Contracts;
using Shared.DataTransferObjects.Product;
using System;
using System.Collections.Concurrent;
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
        private static readonly ConcurrentDictionary<string, object> _locks = new ConcurrentDictionary<string, object>();
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(3, 3); // Giảm từ 5 xuống 3 để giảm tải

        public ProductService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, IConfiguration configuration)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _connectionString = configuration.GetConnectionString("sqlConnection");
        }
        public async Task<List<ProductCreationResult>> CreateProductsBulkAsync(List<ProductForCreationDto> products)
        {
            if (products == null || !products.Any())
            {
                _logger.LogError("Product list is null or empty");
                throw new ArgumentNullException(nameof(products), "Product list cannot be null or empty.");
            }

            _logger.LogInfo($"Starting bulk import of {products.Count} products");
            
            // Acquire semaphore to limit concurrent requests
            await _semaphore.WaitAsync();
            try
            {
                var results = new List<ProductCreationResult>();
                
                // Log thông tin về số lượng sản phẩm
                _logger.LogInfo($"Processing {products.Count} products with {products.Select(p => p.TagID).Distinct().Count()} unique TagIDs");
                
                // Chỉ lấy các TagID duy nhất để giảm số lượng kiểm tra
                var uniqueTagIds = products.Select(p => p.TagID).Distinct().ToList();

                // Kiểm tra trùng TagID hàng loạt - chỉ với các TagID duy nhất
                _logger.LogInfo($"Checking for duplicate TagIDs in database");
                var existingTagIds = await _repository.Product.GetExistingTagIdsAsync(uniqueTagIds);
                var duplicateTagIds = existingTagIds.ToHashSet();
                
                _logger.LogInfo($"Found {duplicateTagIds.Count} duplicate TagIDs out of {uniqueTagIds.Count} unique TagIDs");

                foreach (var product in products)
                {
                    if (duplicateTagIds.Contains(product.TagID))
                    {
                        _logger.LogError($"TagID {product.TagID} already exists, skipping");
                        results.Add(new ProductCreationResult
                        {
                            TagID = product.TagID,
                            IsSuccess = false,
                            ErrorMessage = $"TagID {product.TagID} already exists."
                        });
                        continue;
                    }

                    // Sử dụng lock để tránh race condition khi tạo sản phẩm với cùng TagID
                    var lockObject = _locks.GetOrAdd(product.TagID, new object());
                    lock (lockObject)
                    {
                        try
                        {
                            _logger.LogInfo($"Creating product with TagID: {product.TagID}");
                            var productEntity = _mapper.Map<Product>(product);
                            _repository.Product.CreateProduct(productEntity);
                            var saveResult = _repository.Save(); // Sử dụng Save() thay vì SaveAsync() để đảm bảo atomic
                            _logger.LogInfo($"Successfully saved product with TagID: {product.TagID}, Save result: {saveResult}");

                            results.Add(new ProductCreationResult
                            {
                                TagID = product.TagID,
                                IsSuccess = true,
                                ProductId = productEntity.Id
                            });
                        }
                        catch (DbUpdateException ex)
                        {
                            // Log chi tiết lỗi để dễ debug
                            _logger.LogError($"Database error creating product with TagID {product.TagID}: {ex.InnerException?.Message ?? ex.Message}");
                            _logger.LogError($"Stack trace: {ex.StackTrace}");
                            
                            results.Add(new ProductCreationResult
                            {
                                TagID = product.TagID,
                                IsSuccess = false,
                                ErrorMessage = $"Database error: {ex.InnerException?.Message ?? ex.Message}"
                            });
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Unexpected error creating product with TagID {product.TagID}: {ex.Message}");
                            _logger.LogError($"Stack trace: {ex.StackTrace}");
                            
                            results.Add(new ProductCreationResult
                            {
                                TagID = product.TagID,
                                IsSuccess = false,
                                ErrorMessage = $"Unexpected error: {ex.Message}"
                            });
                        }
                    }
                }
                
                _logger.LogInfo($"Bulk import completed. Success: {results.Count(r => r.IsSuccess)}, Failed: {results.Count(r => !r.IsSuccess)}");

                return results;
            }
            finally
            {
                _semaphore.Release();
                _logger.LogInfo("Released semaphore for bulk import");
            }
        }
        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync(bool trackChanges)
        {
            try
            {
                var products = await _repository.Product.GetAllProductsAsync(trackChanges);
                var productsDto = _mapper.Map<IEnumerable<ProductDto>>(products);
                return productsDto;
            }
            catch (Exception ex)
            {
                return null;
            }
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
                                        Area = reader.GetString("AreaName"),
                                        Address = reader.GetString("Address")
                                    },
                                    Province = reader.GetString("Province"),
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
                return null; // Hoặc throw exception tùy yêu cầu
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

        public async Task<ProductDto> CreateProductsAsync(ProductForCreationDto products)
        {
            if (products == null)
                throw new ArgumentNullException(nameof(products), "Product list cannot be null or empty.");

            var product = _mapper.Map<Product>(products);
             _repository.Product.CreateProduct(product);
            await _repository.SaveAsync();

            var productsToReturn = _mapper.Map<ProductDto>(product);
            return productsToReturn;
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
        public async Task<IEnumerable<ProductExportDto>> GetExportDataAsync(ProductExportQueryDto filter)
        {
            var result = new List<ProductExportDto>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using var command = new MySqlCommand("sp_GetProductExportDetails", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@p_from_date", filter.FromDate.AddDays(1).Date);
                command.Parameters.AddWithValue("@p_to_date", filter.ToDate.AddHours(23).AddMinutes(59).AddSeconds(59));
                command.Parameters.AddWithValue("@p_distributor_id", (object?)filter.DistributorId ?? DBNull.Value);
                command.Parameters.AddWithValue("@p_product_info_id", (object?)filter.ProductInformationId ?? DBNull.Value);
                command.Parameters.AddWithValue("@p_group_by", filter.GroupBy);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    result.Add(new ProductExportDto
                    {
                        TagID = reader["TagID"].ToString(),
                        DistributorName = reader["DistributorName"].ToString(),
                        DistributorCode = reader["DistributorCode"].ToString(),
                        ProductName = reader["ProductName"].ToString(),
                        ProductCode = reader["ProductCode"].ToString(),
                        ShipmentDate = Convert.ToDateTime(reader["ShipmentDate"]),
                        GroupedPeriod = reader["GroupedPeriod"]?.ToString()
                    });
                }
            }

            return result;
        }
        public async Task<byte[]> ExportProductReportAsync(ProductExportQueryDto filter)
        {
            if (filter == null)
            {
                _logger.LogInfo("Filter is null for Product Export Report.");
                return null;
            }

            var data = await GetExportDataAsync(filter);
            if (data == null || !data.Any())
            {
                _logger.LogInfo("No data to export for Product Export Report.");
                return null;
            }

            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "XuatBaoCaoTag.xlsx");

            try
            {
                using (var workbook = new XLWorkbook(templatePath))
                {
                    var worksheet = workbook.Worksheet(1); // Lấy sheet đầu tiên

                    // Fill thông tin thời gian
                    worksheet.Cell("C3").Value = $"Từ ngày: {filter.FromDate:dd/MM/yyyy}";
                    worksheet.Cell("E3").Value = $"đến ngày: {filter.ToDate:dd/MM/yyyy}";
                    worksheet.Cell("B4").Value = data.Count().ToString();
                    // Fill dữ liệu
                    int currentRow = 7;
                    foreach (var item in data)
                    {
                        worksheet.Cell(currentRow, 1).Value = item.TagID;
                        worksheet.Cell(currentRow, 2).Value = item.DistributorName;
                        worksheet.Cell(currentRow, 3).Value = item.DistributorCode;
                        worksheet.Cell(currentRow, 4).Value = item.ProductName;
                        worksheet.Cell(currentRow, 5).Value = item.ProductCode;
                        worksheet.Cell(currentRow, 6).Value = item.ShipmentDate.ToString("dd/MM/yyyy");

                        var range = worksheet.Range(currentRow, 1, currentRow, 6);
                        range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Range(currentRow, 1, currentRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        currentRow++;
                    }

                    // Auto-fit columns
                    worksheet.Columns().AdjustToContents();

                    // Convert to byte array
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        return stream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error exporting Product Export Report: {ex.Message}");
                return null;
            }
        }
    }
}