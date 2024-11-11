using Microsoft.AspNetCore.Mvc;
using Unit.API.ActionFilter;
using Unit.Service.Contracts;
using Unit.Shared.RequestFeatures;

namespace Unit.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public class UserController : ControllerBase
    {
        private readonly IServiceManager _service;

        public UserController(IServiceManager service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetUser([FromQuery] UserParameters userParameters)
        {

            return Ok();
        }
    }
}
