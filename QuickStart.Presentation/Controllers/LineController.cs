using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickStart.Presentation.ActionFilters;
using Service.Contracts;
using Shared.DataTransferObjects.Line;

namespace QuickStart.Presentation.Controllers
{
    [Route("api/lines")]
    [ApiController]
    //[Authorize]
    public class LineController : ControllerBase
    {
        private readonly IServiceManager _service;

        public LineController(IServiceManager service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAllLines()
        {
            var lines = await _service.LineService.GetAllLinesAsync(trackChanges: false);
            return Ok(lines);
        }

        [HttpGet("{lineId:int}", Name = "GetLineById")]
        //[AuthorizePermission("Lines", "View")]
        public async Task<IActionResult> GetLine(int lineId)
        {
            var line = await _service.LineService.GetLineAsync(lineId, trackChanges: false);
            return Ok(line);
        }

        [HttpGet("number/{lineNumber:int}")]
        //AuthorizePermission("Lines", "View")]
        public async Task<IActionResult> GetLineByNumber(int lineNumber)
        {
            var line = await _service.LineService.GetLineByNumberAsync(lineNumber, trackChanges: false);
            return Ok(line);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [AuthorizePermission("Lines", "Create")]
        public async Task<IActionResult> CreateLine([FromBody] LineForCreationDto line)
        {
            var createdLine = await _service.LineService.CreateLineAsync(line);
            return CreatedAtRoute("GetLineById", new { lineId = createdLine.Id }, createdLine);
        }

        [HttpPut("{lineId:int}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [AuthorizePermission("Lines", "Update")]
        public async Task<IActionResult> UpdateLine(int lineId, [FromBody] LineForUpdateDto lineForUpdate)
        {
            await _service.LineService.UpdateLineAsync(lineId, lineForUpdate, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{lineId:int}")]
        [AuthorizePermission("Lines", "Delete")]
        public async Task<IActionResult> DeleteLine(int lineId)
        {
            await _service.LineService.DeleteLineAsync(lineId, trackChanges: false);
            return NoContent();
        }
    }
}