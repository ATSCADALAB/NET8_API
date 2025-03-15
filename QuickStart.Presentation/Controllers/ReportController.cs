using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickStart.Presentation.ActionFilters;
using Service.Contracts;
using Shared.DataTransferObjects.Area;

namespace QuickStart.Presentation.Controllers
{
    [Route("api/reports")]
    [ApiController]
    //[Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IServiceManager _service;

        public ReportController(IServiceManager service) => _service = service;

    }
}