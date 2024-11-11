using Microsoft.AspNetCore.Mvc;
using Unit.API.ActionFilter;
using Unit.Service.Contracts;

namespace Unit.API.Controllers
{
    [Route("api/post")]
    [ApiController]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public class PostController : ControllerBase
    {
        private readonly IServiceManager _service;

        public PostController(IServiceManager service)
        {
            _service = service;
        }


    }
}
