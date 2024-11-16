using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Unit.API.ActionFilter;
using Unit.Entities.ErrorModel;
using Unit.Entities.Exceptions;
using Unit.Entities.Exceptions.Messages;
using Unit.Service.Contracts;
using Unit.Service.Helper;
using Unit.Shared.DataTransferObjects.User;
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

        [HttpGet("p/{id?}")]
        [Authorize]
        public async Task<IActionResult> GetUserById([FromHeader(Name = "Authorization")] string token, [FromQuery] UserParameters userParameters, string id)
        {
            if (userParameters.GetFollowersOrFollowing())
            {
                var user = (IDictionary<string, object>)(await _service.UserService.GetUserByIdAsync(new() { Fields = userParameters.Include }, token, id))!;
                if (user.TryGetValue(userParameters.Include!, out var follower))
                {
                    var follow = (List<string>)follower;
                    if (!follow.Any()) return NotFound(new ErrorDetails()
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = UserExMsg.DoNotHave + $" {userParameters.Include}"
                    });

                    var pagedResult = await _service.UserService.GetUsersByIdsAsync(userParameters, token, follow);
                    Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));
                    return Ok(pagedResult.users);
                }
                else throw new BadRequestException(UserExMsg.DoNotHavePermissionToView);
            }
            else
            {
                var user = (await _service.UserService.GetUserByIdAsync(userParameters, token, id))!;

                return Ok(user);
            }

        }

        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(ImageFileValidationFilter))]
        public async Task<IActionResult> UpdateUser(
            [FromHeader(Name = "Authorization")] string token,
            [FromForm] UserInfoDtoForUpdate userDtoForUpdate,
            [FromForm] IFormFile? imageFile)
        {
            var userId = JwtHelper.GetPayloadData(token, "username");
            string imagePath = null;
            if (imageFile != null && imageFile.Length > 0)
            {
                var tempFilePath = Path.GetTempFileName();

                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                var fileInfo = new FileInfo(tempFilePath);
                imagePath = await _service.UserService.UploadUserImageAsync(userId!, fileInfo);


                fileInfo.Delete();
            }
            await _service.UserService.UpdateUser(userDtoForUpdate, userId!, imagePath);

            return Ok();
        }

    }
}
