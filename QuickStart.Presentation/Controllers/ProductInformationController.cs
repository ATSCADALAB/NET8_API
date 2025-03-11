using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuickStart.Presentation.ActionFilters;
using Service.Contracts;
using Shared.DataTransferObjects.ProductInformation;

namespace QuickStart.Presentation.Controllers
{
    [Route("api/productInformations")]
    [ApiController]
    public class ProductInformationController : ControllerBase
    {
        private readonly IServiceManager _service;

        public ProductInformationController(IServiceManager service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetProductInformations()
        {
            var productInformations = await _service.ProductInformationService.GetAllProductInformationsAsync(trackChanges: false);
            return Ok(productInformations);
        }

        [HttpGet("{id:long}", Name = "ProductInformationById")]
        public async Task<IActionResult> GetProductInformation(long id)
        {
            var productInformation = await _service.ProductInformationService.GetProductInformationAsync(id, trackChanges: false);
            return Ok(productInformation);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateProductInformation([FromBody] ProductInformationForCreationDto productInformation)
        {
            var createdProductInformation = await _service.ProductInformationService.CreateProductInformationAsync(productInformation);
            return CreatedAtRoute("ProductInformationById", new { id = createdProductInformation.Id }, createdProductInformation);
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteProductInformation(long id)
        {
            await _service.ProductInformationService.DeleteProductInformationAsync(id, trackChanges: false);
            return NoContent();
        }

        [HttpPut("{id:long}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateProductInformation(long id, [FromBody] ProductInformationForUpdateDto productInformation)
        {
            await _service.ProductInformationService.UpdateProductInformationAsync(id, productInformation, trackChanges: true);
            return NoContent();
        }
        [HttpPost("import")]
        public async Task<IActionResult> ImportProductInformations(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var workbook = new XLWorkbook(stream)) // Sử dụng ClosedXML để đọc file
                    {
                        var worksheet = workbook.Worksheet(1); // Lấy sheet đầu tiên
                        var rowCount = worksheet.RowsUsed().Count();

                        if (rowCount < 2) // Kiểm tra nếu file rỗng hoặc chỉ có tiêu đề
                            return BadRequest("Excel file is empty or has no data rows.");

                        var productInformations = new List<ProductInformationForCreationDto>();
                        var errors = new List<string>();
                        int successCount = 0;

                        // Bắt đầu từ hàng thứ 2 (hàng 1 là tiêu đề)
                        for (int row = 2; row <= rowCount; row++)
                        {
                            try
                            {
                                // Đọc dữ liệu từ các cột
                                var productInfo = new ProductInformationForCreationDto
                                {
                                    ProductCode = worksheet.Cell(row, 1).GetString()?.Trim(), // ProductCode
                                    ProductName = worksheet.Cell(row, 2).GetString()?.Trim(), // ProductName
                                    Unit = worksheet.Cell(row, 3).GetString()?.Trim(), // Unit
                                    Weight = worksheet.Cell(row, 4).GetValue<decimal>(), // Weight
                                    IsActive = true
                                };

                                // Validate dữ liệu
                                if (string.IsNullOrWhiteSpace(productInfo.ProductCode))
                                    throw new Exception("ProductCode is required.");
                                if (string.IsNullOrWhiteSpace(productInfo.ProductName))
                                    throw new Exception("ProductName is required.");
                                if (string.IsNullOrWhiteSpace(productInfo.Unit))
                                    throw new Exception("Unit is required.");
                                if (productInfo.Weight <= 0)
                                    throw new Exception("Weight must be a positive number.");

                                productInformations.Add(productInfo);
                            }
                            catch (Exception ex)
                            {
                                errors.Add($"Row {row}: {ex.Message}");
                            }
                        }

                        if (productInformations.Count == 0)
                            return BadRequest($"No valid product informations found:\n{string.Join("\n", errors)}");

                        // Gửi danh sách thông tin sản phẩm đến service để lưu
                        foreach (var productInfo in productInformations)
                        {
                            try
                            {
                                await _service.ProductInformationService.CreateProductInformationAsync(productInfo);
                                successCount++;
                            }
                            catch (Exception ex)
                            {
                                errors.Add($"ProductCode '{productInfo.ProductCode}': {ex.Message}");
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
                return BadRequest($"Error importing product informations: {ex.Message}");
            }
        }
        [HttpGet("template")]
        public IActionResult DownloadProductInformationTemplate()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "ProductInformation.xlsx");
            if (!System.IO.File.Exists(filePath))
                return NotFound("Template file not found.");

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ProductInformation.xlsx");
        }
        // GET: api/productInformations/search?name={name}
        [HttpGet("search")]
        public async Task<IActionResult> SearchProductInformations(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Tên tìm kiếm không được để trống.");
            }
            var distributors = await _service.ProductInformationService.GetDistributorsByNameAsync(name);
            return Ok(distributors);
        }
    }
}