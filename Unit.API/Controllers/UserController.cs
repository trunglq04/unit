using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Unit.API.ActionFilter;
using Unit.Entities.ErrorModel;
using Unit.Entities.Exceptions;
using Unit.Entities.Exceptions.Messages;
using Unit.API.ModelBinders;
using Unit.Service.Contracts;
using Unit.Service.Helper;
using Unit.Shared.DataTransferObjects;
using Unit.Shared.RequestFeatures;

namespace Unit.API.Controllers
{
    [Route("api/user")]
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
        [Authorize]
        public async Task<IActionResult> GetUsers([FromHeader(Name = "Authorization")] string token, [FromQuery] UserParameters userParameters)
        {

            var pagedResult = await _service.UserService.GetUsersAsync(userParameters, token);
            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));

            return Ok(pagedResult.users);
        }

        [HttpGet("({ids})", Name = "UserCollection")]
        public async Task<IActionResult> GetUserCollection(
            [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<string> ids, [FromQuery] UserParameters userParameters)
        {
            var pagedResult = await _service.UserService.GetUsersByIdsAsync(userParameters, ids.ToArray());
            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));

            return Ok(pagedResult.users);
        }

        [HttpGet("myProfile")]
        [Authorize]
        public async Task<IActionResult> GetUser([FromHeader(Name = "Authorization")] string token, [FromQuery] UserParameters userParameters)
        {
            var userId = JwtHelper.GetPayloadData(token, "username");

            var user = await _service.UserService.GetUserByIdAsync(userParameters, userId!);

            return Ok(user);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromHeader(Name = "Authorization")] string token, [FromBody] UserInfoDtoForUpdate userDtoForUpdate)
        {
            var userId = JwtHelper.GetPayloadData(token, "username");

            await _service.UserService.UpdateUser(userDtoForUpdate, userId!);

            return Ok();
        }

    }
}
