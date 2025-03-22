using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuickStart.Presentation.ActionFilters;
using Service.Contracts;
using Shared.DataTransferObjects.ProductInformation;

namespace QuickStart.Presentation.Controllers
{
    [Route("api/product-informations")]
    [ApiController]
    //[Authorize]
    public class ProductInformationController : ControllerBase
    {
        private readonly IServiceManager _service;

        public ProductInformationController(IServiceManager service) => _service = service;

        [HttpGet]
        [AuthorizePermission("ProductInformations", "View")]
        public async Task<IActionResult> GetAllProductInformations()
        {
            var productInformations = await _service.ProductInformationService.GetAllProductInformationsAsync(trackChanges: false);
            return Ok(productInformations);
        }

        [HttpGet("{productInformationId:int}", Name = "GetProductInformationById")] 
        public async Task<IActionResult> GetProductInformation(int productInformationId)
        {
            var productInformation = await _service.ProductInformationService.GetProductInformationAsync(productInformationId, trackChanges: false);
            return Ok(productInformation);
        }

        [HttpGet("code/{productCode}")]
        [AuthorizePermission("ProductInformations", "View")]
        public async Task<IActionResult> GetProductInformationByCode(string productCode)
        {
            var productInformation = await _service.ProductInformationService.GetProductInformationByCodeAsync(productCode, trackChanges: false);
            return Ok(productInformation);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [AuthorizePermission("ProductInformations", "Create")]
        public async Task<IActionResult> CreateProductInformation([FromBody] ProductInformationForCreationDto productInformation)
        {
            var createdProductInformation = await _service.ProductInformationService.CreateProductInformationAsync(productInformation);
            return CreatedAtRoute("GetProductInformationById", new { productInformationId = createdProductInformation.Id }, createdProductInformation);
        }

        [HttpPut("{productInformationId:int}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [AuthorizePermission("ProductInformations", "Update")]
        public async Task<IActionResult> UpdateProductInformation(int productInformationId, [FromBody] ProductInformationForUpdateDto productInformationForUpdate)
        {
            await _service.ProductInformationService.UpdateProductInformationAsync(productInformationId, productInformationForUpdate, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{productInformationId:int}")]
        [AuthorizePermission("ProductInformations", "Delete")]
        public async Task<IActionResult> DeleteProductInformation(int productInformationId)
        {
            await _service.ProductInformationService.DeleteProductInformationAsync(productInformationId, trackChanges: false);
            return NoContent();
        }
        [HttpGet("template")]
        public IActionResult DownloadProductInformationTemplate()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "Distributor.xlsx");
            if (!System.IO.File.Exists(filePath))
                return NotFound("Template file not found.");

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Distributor.xlsx");
        }
        [HttpPost("import")]
        [AuthorizePermission("ProductInformations", "Create")]
        public async Task<IActionResult> ImportProductInformations(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheet(1); // Lấy sheet đầu tiên
                        if (worksheet == null)
                            return BadRequest("Excel file has no valid worksheet.");

                        var rowCount = worksheet.RowsUsed().Count();
                        if (rowCount < 2)
                            return BadRequest("Excel file is empty or has no data rows.");

                        var productInformations = new List<ProductInformationForCreationDto>();
                        var errors = new List<string>();
                        int successCount = 0;

                        // Đọc dữ liệu từ Excel
                        for (int row = 2; row <= rowCount; row++)
                        {
                            try
                            {
                                var product = new ProductInformationForCreationDto
                                {
                                    ProductCode = worksheet.Cell(row, 1).GetString()?.Trim(), // Mã Sản Phẩm
                                    ProductName = worksheet.Cell(row, 2).GetString()?.Trim(), // Tên Sản Phẩm
                                    Unit = worksheet.Cell(row, 3).GetString()?.Trim(), // Đơn Vị
                                    WeightPerUnit = worksheet.Cell(row, 4).GetValue<decimal>(), // QC (WeightPerUnit)
                                    IsActive = true // Mặc định
                                };

                                // Kiểm tra dữ liệu bắt buộc
                                if (string.IsNullOrWhiteSpace(product.ProductCode) ||
                                    string.IsNullOrWhiteSpace(product.ProductName) ||
                                    string.IsNullOrWhiteSpace(product.Unit) ||
                                    product.WeightPerUnit < 0)
                                {
                                    throw new Exception("Missing or invalid required fields (ProductCode, ProductName, Unit, WeightPerUnit).");
                                }

                                productInformations.Add(product);
                            }
                            catch (Exception ex)
                            {
                                errors.Add($"Row {row}: {ex.Message}");
                            }
                        }

                        if (productInformations.Count == 0)
                            return BadRequest($"No valid product informations found:\n{string.Join("\n", errors)}");

                        const int batchSize = 100; // Xử lý theo lô 100 bản ghi
                        for (int i = 0; i < productInformations.Count; i += batchSize)
                        {
                            var batch = productInformations.Skip(i).Take(batchSize).ToList();
                            try
                            {
                                foreach (var product in batch)
                                {
                                    await _service.ProductInformationService.CreateProductInformationAsync(product);
                                    successCount++;
                                }
                            }
                            catch (Exception ex)
                            {
                                errors.Add($"Batch {i / batchSize + 1}: {ex.Message} - Inner: {ex.InnerException?.Message}");
                            }
                        }

                        var result = new
                        {
                            SuccessCount = successCount,
                            Errors = errors
                        };

                        if (errors.Count > 0)
                            return BadRequest(result);

                        return Ok(result);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error importing product informations: {ex.Message} - StackTrace: {ex.StackTrace}");
            }
        }
    }
}