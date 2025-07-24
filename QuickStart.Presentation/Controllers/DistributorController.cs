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
        //[AuthorizePermission("Distributors", "View")]
        public async Task<IActionResult> GetAllDistributors()
        {
            var distributors = await _service.DistributorService.GetAllDistributorsAsync(trackChanges: false);
            return Ok(distributors);
        }

        [HttpGet("{distributorId:int}", Name = "GetDistributorById")]
        public async Task<IActionResult> GetDistributor(int distributorId)
        {
            var distributor = await _service.DistributorService.GetDistributorAsync(distributorId, trackChanges: false);
            return Ok(distributor);
        }

        [HttpGet("code/{distributorCode}")]
        //[AuthorizePermission("Distributors", "View")]
        public async Task<IActionResult> GetDistributorByCode(string distributorCode)
        {
            var distributor = await _service.DistributorService.GetDistributorByCodeAsync(distributorCode, trackChanges: false);
            return Ok(distributor);
        }

        [HttpGet("by-area/{areaId:int}")]
        //[AuthorizePermission("Distributors", "View")]
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
        // Hàm Normalize để chuyển chuỗi về chữ thường, xoá khoảng trống để import
        private string Normalize(string input)
        {
            return string.IsNullOrWhiteSpace(input)
                ? string.Empty
                : string.Join(" ", input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                    .ToLowerInvariant();
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
                                var normalizedAreaName = Normalize(areaName);
                                var area = areas.FirstOrDefault(a => Normalize(a.AreaName) == normalizedAreaName);
                                int? areaId = area?.Id;

                                var distributor = new DistributorForCreationDto
                                {
                                    DistributorCode = worksheet.Cell(row, 1).GetString()?.Trim(), // Mã NPP
                                    DistributorName = worksheet.Cell(row, 3).GetString()?.Trim(), // Tên ĐT Thuế GTGT (Update 01/04/2025)
                                    Address = worksheet.Cell(row, 4).GetString()?.Trim(),
                                    Province = worksheet.Cell(row, 5).GetString()?.Trim(), // Tỉnh thành
                                    AreaId = areaId ?? 0, // Nếu không có area thì để 0 hoặc bỏ qua tùy logic backend
                                    IsActive = true // Giá trị mặc định
                                };

                                // Kiểm tra dữ liệu cơ bản
                                if (string.IsNullOrWhiteSpace(distributor.DistributorCode) ||
                                    string.IsNullOrWhiteSpace(distributor.DistributorName))
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
                                    try
                                    {   
                                        var distributor1 = await _service.DistributorService.GetDistributorByCodeAsync(distributor.DistributorCode, trackChanges: false);
                                        if (distributor1!=null)
                                        {
                                            errors.Add($"Distributor Code is exist :{distributor1.DistributorCode}");
                                        }
                                        else
                                        {
                                            await _service.DistributorService.CreateDistributorAsync(distributor);
                                            successCount++;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        await _service.DistributorService.CreateDistributorAsync(distributor);
                                        successCount++;
                                    }

                                }
                            }
                            catch (Exception ex)
                            {
                                
                            }
                        }

                        var result = new
                        {
                            SuccessCount = successCount,
                            Errors = errors
                        };


                        return Ok(result);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error importing distributors: {ex.Message}");
            }
        }
        [HttpGet("export")]
        public async Task<IActionResult> ExportDistributors()
        {
            try
            {
                // Lấy tất cả distributors
                var distributors = await _service.DistributorService.GetAllDistributorsAsync(trackChanges: false);

                // Lấy tất cả areas để map areaId với tên khu vực
                var areas = await _service.AreaService.GetAllAreasAsync(trackChanges: false);

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Distributors");

                    // Tạo header
                    worksheet.Cell(1, 1).Value = "Mã NPP";
                    worksheet.Cell(1, 2).Value = "Tên nhà PP";
                    worksheet.Cell(1, 3).Value = "Tên ĐT thuế GTGT";
                    worksheet.Cell(1, 4).Value = "Địa chỉ";
                    worksheet.Cell(1, 5).Value = "Tỉnh Thành";
                    worksheet.Cell(1, 6).Value = "Khu Vực";

                    // Format header
                    var headerRange = worksheet.Range(1, 1, 1, 6);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    // Điền dữ liệu
                    int currentRow = 2;
                    foreach (var distributor in distributors)
                    {
                        // Tìm tên khu vực từ areaId
                        var area = areas.FirstOrDefault(a => a.Id == distributor.AreaId);

                        worksheet.Cell(currentRow, 1).Value = distributor.DistributorCode ?? "";
                        worksheet.Cell(currentRow, 2).Value = distributor.DistributorName ?? ""; // Tên nhà PP
                        worksheet.Cell(currentRow, 3).Value = distributor.DistributorName ?? ""; // Tên ĐT thuế GTGT (có thể khác hoặc giống)
                        worksheet.Cell(currentRow, 4).Value = distributor.Address ?? "";
                        worksheet.Cell(currentRow, 5).Value = distributor.Province ?? "";
                        worksheet.Cell(currentRow, 6).Value = area?.AreaName ?? "";

                        currentRow++;
                    }

                    // Auto-fit columns
                    worksheet.Columns().AdjustToContents();

                    // Tạo file trong memory
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var fileBytes = stream.ToArray();

                        // Tạo tên file với timestamp
                        var fileName = $"Distributors_Export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                        return File(fileBytes,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error exporting distributors: {ex.Message}");
            }
        }
    }
}