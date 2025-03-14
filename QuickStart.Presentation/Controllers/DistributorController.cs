using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuickStart.Presentation.ActionFilters;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.DataTransferObjects.Distributor;
using Shared.DataTransferObjects.ProductInformation;

namespace QuickStart.Presentation.Controllers
{
    [Route("api/distributors")]
    [ApiController]
    public class DistributorController : ControllerBase
    {
        private readonly IServiceManager _service;

        public DistributorController(IServiceManager service) => _service = service;
        [HttpPost("import")]
        [AuthorizePermission("Distributors", "Create")]
        public async Task<IActionResult> ImportDistributors(IFormFile file)
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

                        var distributors = new List<DistributorForCreationDto>();
                        var errors = new List<string>();
                        int successCount = 0;

                        // Bắt đầu từ hàng thứ 2 (hàng 1 là tiêu đề)
                        for (int row = 2; row <= rowCount; row++)
                        {
                            try
                            {
                                // Đọc dữ liệu từ các cột
                                var distributor = new DistributorForCreationDto
                                {
                                    DistributorCode = worksheet.Cell(row, 1).GetString()?.Trim(), // Mã NPP
                                    DistributorName = worksheet.Cell(row, 2).GetString()?.Trim(), // Tên nhà PP
                                    Address = worksheet.Cell(row, 3).GetString()?.Trim(), // Địa chỉ
                                    PhoneNumber = worksheet.Cell(row, 4).GetString()?.Trim(), // Số điện thoại
                                    ContactSource = worksheet.Cell(row, 5).GetString()?.Trim(), // Nguồn liên hệ
                                    Area = worksheet.Cell(row, 6).GetString()?.Trim(), // Khu vực
                                    Note = worksheet.Cell(row, 7).GetString()?.Trim(), // Ghi chú
                                    IsActive = true // Giá trị mặc định
                                };
                                distributors.Add(distributor);
                            }
                            catch (Exception ex)
                            {
                                errors.Add($"Row {row}: {ex.Message}");
                            }
                        }

                        if (distributors.Count == 0)
                            return BadRequest($"No valid distributors found:\n{string.Join("\n", errors)}");

                        const int batchSize = 100;
                        for (int i = 0; i < distributors.Count; i += batchSize)
                        {
                            var batch = distributors.Skip(i).Take(batchSize).ToList();
                            try
                            {
                                // Sử dụng Bulk Insert hoặc vòng lặp bình thường
                                foreach (var distributor in batch)
                                {
                                    await _service.DistributorService.CreateDistributorAsync(distributor);
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
                return BadRequest($"Error importing distributors: ${ex.Message}");
            }
        }
        
        [HttpGet]

        public async Task<IActionResult> GetDistributors()
        {
            var distributors = await _service.DistributorService.GetAllDistributorsAsync(trackChanges: false);
            return Ok(distributors);
        }

        [HttpGet("{id:long}", Name = "DistributorById")]

        public async Task<IActionResult> GetDistributor(long id)
        {
            var distributor = await _service.DistributorService.GetDistributorAsync(id, trackChanges: false);
            return Ok(distributor);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [AuthorizePermission("Distributors", "Create")]
        public async Task<IActionResult> CreateDistributor([FromBody] DistributorForCreationDto distributor)
        {
            var createdDistributor = await _service.DistributorService.CreateDistributorAsync(distributor);
            return CreatedAtRoute("DistributorById", new { id = createdDistributor.Id }, createdDistributor);
        }

        [HttpDelete("{id:long}")]
        [AuthorizePermission("Distributors", "Delete")]
        public async Task<IActionResult> DeleteDistributor(long id)
        {
            await _service.DistributorService.DeleteDistributorAsync(id, trackChanges: false);
            return NoContent();
        }

        [HttpPut("{id:long}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [AuthorizePermission("Distributors", "Delete")]
        public async Task<IActionResult> UpdateDistributor(long id, [FromBody] DistributorForUpdateDto distributor)
        {
            await _service.DistributorService.UpdateDistributorAsync(id, distributor, trackChanges: true);
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
        // GET: api/productInformations/search?name={name}
        [HttpGet("search")]

        public async Task<IActionResult> SearchProductInformations(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Tên tìm kiếm không được để trống.");
            }
            var distributors = await _service.DistributorService.GetDistributorsByNameAsync(name);
            return Ok(distributors);
        }
    }
}