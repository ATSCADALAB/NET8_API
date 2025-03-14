using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickStart.Presentation.ActionFilters;
using Service.Contracts;
using Shared.DataTransferObjects.InboundRecord;

namespace QuickStart.Presentation.Controllers
{
    [Route("api/inbound-records")]
    [ApiController]
    //[Authorize]
    public class InboundRecordController : ControllerBase
    {
        private readonly IServiceManager _service;

        public InboundRecordController(IServiceManager service) => _service = service;

        [HttpGet]
        [AuthorizePermission("InboundRecords", "View")]
        public async Task<IActionResult> GetAllInboundRecords()
        {
            var inboundRecords = await _service.InboundRecordService.GetAllInboundRecordsAsync(trackChanges: false);
            return Ok(inboundRecords);
        }

        [HttpGet("{inboundRecordId:int}", Name = "GetInboundRecordById")]
        [AuthorizePermission("InboundRecords", "View")]
        public async Task<IActionResult> GetInboundRecord(int inboundRecordId)
        {
            var inboundRecord = await _service.InboundRecordService.GetInboundRecordAsync(inboundRecordId, trackChanges: false);
            return Ok(inboundRecord);
        }

        [HttpGet("by-product/{productInformationId:int}")]
        [AuthorizePermission("InboundRecords", "View")]
        public async Task<IActionResult> GetInboundRecordsByProductInformation(int productInformationId)
        {
            var inboundRecords = await _service.InboundRecordService.GetInboundRecordsByProductInformationAsync(productInformationId, trackChanges: false);
            return Ok(inboundRecords);
        }

        [HttpGet("by-date")]
        [AuthorizePermission("InboundRecords", "View")]
        public async Task<IActionResult> GetInboundRecordsByDate([FromQuery] DateTime inboundDate)
        {
            var inboundRecords = await _service.InboundRecordService.GetInboundRecordsByDateAsync(inboundDate, trackChanges: false);
            return Ok(inboundRecords);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [AuthorizePermission("InboundRecords", "Create")]
        public async Task<IActionResult> CreateInboundRecord([FromBody] InboundRecordForCreationDto inboundRecord)
        {
            var createdInboundRecord = await _service.InboundRecordService.CreateInboundRecordAsync(inboundRecord);
            return CreatedAtRoute("GetInboundRecordById", new { inboundRecordId = createdInboundRecord.Id }, createdInboundRecord);
        }

        [HttpPut("{inboundRecordId:int}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [AuthorizePermission("InboundRecords", "Update")]
        public async Task<IActionResult> UpdateInboundRecord(int inboundRecordId, [FromBody] InboundRecordForUpdateDto inboundRecordForUpdate)
        {
            await _service.InboundRecordService.UpdateInboundRecordAsync(inboundRecordId, inboundRecordForUpdate, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{inboundRecordId:int}")]
        [AuthorizePermission("InboundRecords", "Delete")]
        public async Task<IActionResult> DeleteInboundRecord(int inboundRecordId)
        {
            await _service.InboundRecordService.DeleteInboundRecordAsync(inboundRecordId, trackChanges: false);
            return NoContent();
        }
    }
}