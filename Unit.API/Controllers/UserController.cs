using Microsoft.AspNetCore.Mvc;
using Unit.Service.Contracts;

namespace Unit.API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IServiceManager _service;

        public UserController(IServiceManager service)
        {
            _service = service;
        }


    }
}
