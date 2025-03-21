using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickStart.Presentation.ActionFilters;
using Service.Contracts;
using Shared.DataTransferObjects.SensorRecord;

namespace QuickStart.Presentation.Controllers
{
    [Route("api/sensor-records")]
    [ApiController]
    //[Authorize]
    public class SensorRecordController : ControllerBase
    {
        private readonly IServiceManager _service;

        public SensorRecordController(IServiceManager service) => _service = service;

        [HttpGet]
        [AuthorizePermission("SensorRecords", "View")]
        public async Task<IActionResult> GetAllSensorRecords()
        {
            var sensorRecords = await _service.SensorRecordService.GetAllSensorRecordsAsync(trackChanges: false);
            return Ok(sensorRecords);
        }

        [HttpGet("{sensorRecordId:int}", Name = "GetSensorRecordById")]
        [AuthorizePermission("SensorRecords", "View")]
        public async Task<IActionResult> GetSensorRecord(int sensorRecordId)
        {
            var sensorRecord = await _service.SensorRecordService.GetSensorRecordAsync(sensorRecordId, trackChanges: false);
            return Ok(sensorRecord);
        }

        [HttpGet("by-order/{orderId:guid}")]
        //[AuthorizePermission("SensorRecords", "View")]
        public async Task<IActionResult> GetSensorRecordsByOrder(Guid orderId)
        {
            var sensorRecords = await _service.SensorRecordService.GetSensorRecordsByOrderAsync(orderId, trackChanges: false);
            return Ok(sensorRecords);
        }

        [HttpGet("by-order-detail/{orderDetailId:int}")]
        [AuthorizePermission("SensorRecords", "View")]
        public async Task<IActionResult> GetSensorRecordsByOrderDetail(int orderDetailId)
        {
            var sensorRecords = await _service.SensorRecordService.GetSensorRecordsByOrderDetailAsync(orderDetailId, trackChanges: false);
            return Ok(sensorRecords);
        }

        [HttpGet("by-line/{lineId:int}")]
        [AuthorizePermission("SensorRecords", "View")]
        public async Task<IActionResult> GetSensorRecordsByLine(int lineId)
        {
            var sensorRecords = await _service.SensorRecordService.GetSensorRecordsByLineAsync(lineId, trackChanges: false);
            return Ok(sensorRecords);
        }

        [HttpGet("by-date")]
        [AuthorizePermission("SensorRecords", "View")]
        public async Task<IActionResult> GetSensorRecordsByDate([FromQuery] DateTime recordDate)
        {
            var sensorRecords = await _service.SensorRecordService.GetSensorRecordsByDateAsync(recordDate, trackChanges: false);
            return Ok(sensorRecords);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        //[AuthorizePermission("SensorRecords", "Create")]
        public async Task<IActionResult> CreateSensorRecord([FromBody] SensorRecordForCreationDto sensorRecord)
        {
            var createdSensorRecord = await _service.SensorRecordService.CreateSensorRecordAsync(sensorRecord);
            return CreatedAtRoute("GetSensorRecordById", new { sensorRecordId = createdSensorRecord.Id }, createdSensorRecord);
        }

        [HttpPut("{sensorRecordId:int}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [AuthorizePermission("SensorRecords", "Update")]
        public async Task<IActionResult> UpdateSensorRecord(int sensorRecordId, [FromBody] SensorRecordForUpdateDto sensorRecordForUpdate)
        {
            await _service.SensorRecordService.UpdateSensorRecordAsync(sensorRecordId, sensorRecordForUpdate, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{sensorRecordId:int}")]
        [AuthorizePermission("SensorRecords", "Delete")]
        public async Task<IActionResult> DeleteSensorRecord(int sensorRecordId)
        {
            await _service.SensorRecordService.DeleteSensorRecordAsync(sensorRecordId, trackChanges: false);
            return NoContent();
        }
    }
}