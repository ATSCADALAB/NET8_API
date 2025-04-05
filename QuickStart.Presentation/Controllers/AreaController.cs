using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickStart.Presentation.ActionFilters;
using Service.Contracts;
using Shared.DataTransferObjects.Area;

namespace QuickStart.Presentation.Controllers
{
    [Route("api/areas")]
    [ApiController]
    //[Authorize]
    public class AreaController : ControllerBase
    {
        private readonly IServiceManager _service;

        public AreaController(IServiceManager service) => _service = service;

        [HttpGet]
        //[AuthorizePermission("Areas", "View")]
        public async Task<IActionResult> GetAllAreas()
        {
            var areas = await _service.AreaService.GetAllAreasAsync(trackChanges: false);
            return Ok(areas);
        }

        [HttpGet("{areaId:int}", Name = "GetAreaById")]
        //[AuthorizePermission("Areas", "View")]
        public async Task<IActionResult> GetArea(int areaId)
        {
            var area = await _service.AreaService.GetAreaAsync(areaId, trackChanges: false);
            return Ok(area);
        }

        [HttpGet("code/{areaCode}")]
        //[AuthorizePermission("Areas", "View")]
        public async Task<IActionResult> GetAreaByCode(string areaCode)
        {
            var area = await _service.AreaService.GetAreaByCodeAsync(areaCode, trackChanges: false);
            return Ok(area);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [AuthorizePermission("Areas", "Create")]
        public async Task<IActionResult> CreateArea([FromBody] AreaForCreationDto area)
        {
            var createdArea = await _service.AreaService.CreateAreaAsync(area);
            return CreatedAtRoute("GetAreaById", new { areaId = createdArea.Id }, createdArea);
        }

        [HttpPut("{areaId:int}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [AuthorizePermission("Areas", "Update")]
        public async Task<IActionResult> UpdateArea(int areaId, [FromBody] AreaForUpdateDto areaForUpdate)
        {
            await _service.AreaService.UpdateAreaAsync(areaId, areaForUpdate, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{areaId:int}")]
        [AuthorizePermission("Areas", "Delete")]
        public async Task<IActionResult> DeleteArea(int areaId)
        {
            await _service.AreaService.DeleteAreaAsync(areaId, trackChanges: false);
            return NoContent();
        }
    }
}