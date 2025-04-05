using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickStart.Presentation.ActionFilters;
using Service.Contracts;
using Shared.DataTransferObjects.Stock;
using System.Threading.Tasks;

namespace QuickStart.Presentation.Controllers
{
    [Route("api/stock")]
    [ApiController]
    //[Authorize]
    public class StockController : ControllerBase
    {
        private readonly IServiceManager _service;

        public StockController(IServiceManager service) => _service = service;

        [HttpGet]
        [AuthorizePermission("Stock", "View")]
        public async Task<IActionResult> GetAllStocks()
        {
            var stocks = await _service.StockService.GetAllStocksAsync(trackChanges: false);
            return Ok(stocks);
        }

        [HttpGet("{stockId:int}", Name = "GetStockById")]
        //[AuthorizePermission("Stock", "View")]
        public async Task<IActionResult> GetStock(int stockId)
        {
            var stock = await _service.StockService.GetStockAsync(stockId, trackChanges: false);
            return Ok(stock);
        }

        [HttpGet("by-product/{productInformationId:int}")]
        //[AuthorizePermission("Stock", "View")]
        public async Task<IActionResult> GetStockByProductInformation(int productInformationId)
        {
            var stock = await _service.StockService.GetStockByProductInformationAsync(productInformationId, trackChanges: false);
            return Ok(stock);
        }

        [HttpPost]
        //[ServiceFilter(typeof(ValidationFilterAttribute))]
        //[AuthorizePermission("Stock", "Create")]
        public async Task<IActionResult> CreateStock([FromBody] StockForCreationDto stock)
        {
            var createdStock = await _service.StockService.CreateStockAsync(stock);
            return CreatedAtRoute("GetStockById", new { stockId = createdStock.Id }, createdStock);
        }

        [HttpPut("{stockId:int}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        //[AuthorizePermission("Stock", "Update")]
        public async Task<IActionResult> UpdateStock(int stockId, [FromBody] StockForUpdateDto stockForUpdate)
        {
            await _service.StockService.UpdateStockAsync(stockId, stockForUpdate, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{stockId:int}")]
        //[AuthorizePermission("Stock", "Delete")]
        public async Task<IActionResult> DeleteStock(int stockId)
        {
            await _service.StockService.DeleteStockAsync(stockId, trackChanges: false);
            return NoContent();
        }

        [HttpGet("report/daily")]
        //[AuthorizePermission("Stock", "View")]
        public async Task<IActionResult> GetDailyInventoryReport([FromQuery] DateTime date)
        {
            var report = await _service.StockService.GetDailyInventoryReportAsync(date);
            return Ok(report);
        }

        [HttpGet("report/monthly")]
        //[AuthorizePermission("Stock", "View")]
        public async Task<IActionResult> GetMonthlyInventoryReport([FromQuery] int year, [FromQuery] int month)
        {
            var report = await _service.StockService.GetMonthlyInventoryReportAsync(year, month);
            return Ok(report);
        }

        [HttpGet("report/yearly")]
        //[AuthorizePermission("Stock", "View")]
        public async Task<IActionResult> GetYearlyInventoryReport([FromQuery] int year)
        {
            var report = await _service.StockService.GetYearlyInventoryReportAsync(year);
            return Ok(report);
        }
    }
}