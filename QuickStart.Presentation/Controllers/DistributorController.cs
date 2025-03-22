using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuickStart.Presentation.ActionFilters;
using Service.Contracts;
using Shared.DataTransferObjects.Distributor;

namespace QuickStart.Presentation.Controllers
{
    [Route("api/distributors")]
    [ApiController]
    //[Authorize]
    public class DistributorController : ControllerBase
    {
        private readonly IServiceManager _service;

        public DistributorController(IServiceManager service) => _service = service;

        [HttpGet]
        [AuthorizePermission("Distributors", "View")]
        public async Task<IActionResult> GetAllDistributors()
        {
            var distributors = await _service.DistributorService.GetAllDistributorsAsync(trackChanges: false);
            return Ok(distributors);
        }

        [HttpGet("{distributorId:int}", Name = "GetDistributorById")]
        [AuthorizePermission("Distributors", "View")]
        public async Task<IActionResult> GetDistributor(int distributorId)
        {
            var distributor = await _service.DistributorService.GetDistributorAsync(distributorId, trackChanges: false);
            return Ok(distributor);
        }

        [HttpGet("code/{distributorCode}")]
        [AuthorizePermission("Distributors", "View")]
        public async Task<IActionResult> GetDistributorByCode(string distributorCode)
        {
            var distributor = await _service.DistributorService.GetDistributorByCodeAsync(distributorCode, trackChanges: false);
            return Ok(distributor);
        }

        [HttpGet("by-area/{areaId:int}")]
        [AuthorizePermission("Distributors", "View")]
        public async Task<IActionResult> GetDistributorsByArea(int areaId)
        {
            var distributors = await _service.DistributorService.GetDistributorsByAreaAsync(areaId, trackChanges: false);
            return Ok(distributors);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [AuthorizePermission("Distributors", "Create")]
        public async Task<IActionResult> CreateDistributor([FromBody] DistributorForCreationDto distributor)
        {
            var createdDistributor = await _service.DistributorService.CreateDistributorAsync(distributor);
            return CreatedAtRoute("GetDistributorById", new { distributorId = createdDistributor.Id }, createdDistributor);
        }

        [HttpPut("{distributorId:int}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [AuthorizePermission("Distributors", "Update")]
        public async Task<IActionResult> UpdateDistributor(int distributorId, [FromBody] DistributorForUpdateDto distributorForUpdate)
        {
            await _service.DistributorService.UpdateDistributorAsync(distributorId, distributorForUpdate, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{distributorId:int}")]
        [AuthorizePermission("Distributors", "Delete")]
        public async Task<IActionResult> DeleteDistributor(int distributorId)
        {
            await _service.DistributorService.DeleteDistributorAsync(distributorId, trackChanges: false);
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
        //[ServiceFilter(typeof(ValidationFilterAttribute))]
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
                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheet(1); // Lấy sheet đầu tiên
                        var rowCount = worksheet.RowsUsed().Count();

                        if (rowCount < 2) // Kiểm tra nếu file rỗng hoặc chỉ có tiêu đề
                            return BadRequest("Excel file is empty or has no data rows.");

                        var distributors = new List<DistributorForCreationDto>();
                        var errors = new List<string>();
                        int successCount = 0;

                        // Lấy tất cả khu vực để map tên khu vực với areaId
                        var areas = await _service.AreaService.GetAllAreasAsync(trackChanges: false);

                        // Bắt đầu từ hàng thứ 2 (hàng 1 là tiêu đề)
                        for (int row = 2; row <= rowCount; row++)
                        {
                            try
                            {
                                var areaName = worksheet.Cell(row, 6).GetString()?.Trim();
                                var area = areas.FirstOrDefault(a => a.AreaName == areaName);
                                int? areaId = area?.Id; // Nếu không tìm thấy area thì để null

                                var distributor = new DistributorForCreationDto
                                {
                                    DistributorCode = worksheet.Cell(row, 1).GetString()?.Trim(), // Mã NPP
                                    DistributorName = worksheet.Cell(row, 2).GetString()?.Trim(), // Tên nhà PP
                                    Address = worksheet.Cell(row, 3).GetString()?.Trim(), // Địa chỉ
                                    PhoneNumber = worksheet.Cell(row, 4).GetString()?.Trim(), // Số điện thoại
                                    ContactSource = worksheet.Cell(row, 5).GetString()?.Trim(), // Người liên hệ
                                    AreaId = areaId ?? 0, // Nếu không có area thì để 0 hoặc bỏ qua tùy logic backend
                                    IsActive = true // Giá trị mặc định
                                };

                                // Kiểm tra dữ liệu cơ bản
                                if (string.IsNullOrWhiteSpace(distributor.DistributorCode) ||
                                    string.IsNullOrWhiteSpace(distributor.DistributorName) ||
                                    string.IsNullOrWhiteSpace(distributor.PhoneNumber))
                                {
                                    throw new Exception("Missing required fields (DistributorCode, DistributorName, PhoneNumber).");
                                }

                                distributors.Add(distributor);
                            }
                            catch (Exception ex)
                            {
                                errors.Add($"Row {row}: {ex.Message}");
                            }
                        }

                        if (distributors.Count == 0)
                            return BadRequest($"No valid distributors found:\n{string.Join("\n", errors)}");

                        const int batchSize = 100; // Xử lý theo lô 100 bản ghi
                        for (int i = 0; i < distributors.Count; i += batchSize)
                        {
                            var batch = distributors.Skip(i).Take(batchSize).ToList();
                            try
                            {
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
                return BadRequest($"Error importing distributors: {ex.Message}");
            }
        }
    }
}