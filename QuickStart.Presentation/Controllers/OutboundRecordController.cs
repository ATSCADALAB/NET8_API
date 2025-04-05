using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuickStart.Presentation.ActionFilters;
using Service.Contracts;
using Shared.DataTransferObjects.OutboundRecord;
using System.Threading.Tasks;

namespace QuickStart.Presentation.Controllers
{
    [Route("api/outbound-records")]
    [ApiController]
    //[Authorize]
    public class OutboundRecordController : ControllerBase
    {
        private readonly IServiceManager _service;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public OutboundRecordController(IServiceManager service, IHttpContextAccessor contextAccessor)
        {
            _service = service;
            _httpContextAccessor = contextAccessor;
        }

        [HttpGet]
        //[AuthorizePermission("OutboundRecords", "View")]
        public async Task<IActionResult> GetAllOutboundRecords()
        {
            var outboundRecords = await _service.OutboundRecordService.GetAllOutboundRecordsAsync(trackChanges: false);
            return Ok(outboundRecords);
        }

        [HttpGet("{outboundRecordId:int}", Name = "GetOutboundRecordById")]
        //[AuthorizePermission("OutboundRecords", "View")]
        public async Task<IActionResult> GetOutboundRecord(int outboundRecordId)
        {
            var outboundRecord = await _service.OutboundRecordService.GetOutboundRecordAsync(outboundRecordId, trackChanges: false);
            return Ok(outboundRecord);
        }

        [HttpGet("by-product/{productInformationId:int}")]
        //[AuthorizePermission("OutboundRecords", "View")]
        public async Task<IActionResult> GetOutboundRecordsByProductInformation(int productInformationId)
        {
            var outboundRecords = await _service.OutboundRecordService.GetOutboundRecordsByProductInformationAsync(productInformationId, trackChanges: false);
            return Ok(outboundRecords);
        }

        [HttpGet("by-date")]
        //[AuthorizePermission("OutboundRecords", "View")]
        public async Task<IActionResult> GetOutboundRecordsByDate([FromQuery] DateTime outboundDate)
        {
            var outboundRecords = await _service.OutboundRecordService.GetOutboundRecordsByDateAsync(outboundDate, trackChanges: false);
            return Ok(outboundRecords);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        //[AuthorizePermission("OutboundRecords", "Create")]
        public async Task<IActionResult> CreateOutboundRecord([FromBody] OutboundRecordForCreationDto outboundRecord)
        {
            var createdOutboundRecord = await _service.OutboundRecordService.CreateOutboundRecordAsync(outboundRecord,_httpContextAccessor);
            return CreatedAtRoute("GetOutboundRecordById", new { outboundRecordId = createdOutboundRecord.Id }, createdOutboundRecord);
        }
    }
}