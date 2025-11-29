using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickStart.Presentation.ActionFilters;
using Service.Contracts;
using Shared.DataTransferObjects.BagWeightInfo;

namespace QuickStart.Presentation.Controllers
{
    [Route("api/bag-weight-infos")]
    [ApiController]
    //[Authorize]
    public class BagWeightInfoController : ControllerBase
    {
        private readonly IServiceManager _service;

        public BagWeightInfoController(IServiceManager service) => _service = service;

        [HttpGet]
        //[AuthorizePermission("BagWeightInfos", "View")]
        public async Task<IActionResult> GetAllBagWeightInfos()
        {
            var bagWeightInfos = await _service.BagWeightInfoService.GetAllBagWeightInfosAsync(trackChanges: false);
            return Ok(bagWeightInfos);
        }

        [HttpGet("{bagWeightInfoId:int}", Name = "GetBagWeightInfoById")]
        public async Task<IActionResult> GetBagWeightInfo(int bagWeightInfoId)
        {
            var bagWeightInfo = await _service.BagWeightInfoService.GetBagWeightInfoAsync(bagWeightInfoId, trackChanges: false);
            return Ok(bagWeightInfo);
        }

        [HttpGet("weight/{weight:double}")]
        //[AuthorizePermission("BagWeightInfos", "View")]
        public async Task<IActionResult> GetBagWeightInfoByWeight(double weight)
        {
            var bagWeightInfo = await _service.BagWeightInfoService.GetBagWeightInfoByWeightAsync(weight, trackChanges: false,1);
            return Ok(bagWeightInfo);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        //[AuthorizePermission("BagWeightInfos", "Create")]
        public async Task<IActionResult> CreateBagWeightInfo([FromBody] BagWeightInfoForCreationDto bagWeightInfo)
        {
            var createdBagWeightInfo = await _service.BagWeightInfoService.CreateBagWeightInfoAsync(bagWeightInfo);
            return CreatedAtRoute("GetBagWeightInfoById", new { bagWeightInfoId = createdBagWeightInfo.Id }, createdBagWeightInfo);
        }

        [HttpPut("{bagWeightInfoId:int}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        //[AuthorizePermission("BagWeightInfos", "Update")]
        public async Task<IActionResult> UpdateBagWeightInfo(int bagWeightInfoId, [FromBody] BagWeightInfoForUpdateDto bagWeightInfoForUpdate)
        {
            await _service.BagWeightInfoService.UpdateBagWeightInfoAsync(bagWeightInfoId, bagWeightInfoForUpdate, trackChanges: true, bagWeightInfoForUpdate.LineID);
            return NoContent();
        }

        [HttpDelete("{bagWeightInfoId:int}")]
        //[AuthorizePermission("BagWeightInfos", "Delete")]
        public async Task<IActionResult> DeleteBagWeightInfo(int bagWeightInfoId)
        {
            await _service.BagWeightInfoService.DeleteBagWeightInfoAsync(bagWeightInfoId, trackChanges: false);
            return NoContent();
        }
    }
}