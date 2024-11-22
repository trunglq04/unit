using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Unit.API.ActionFilter;
using Unit.Service.Contracts;
using Unit.Shared.DataTransferObjects.Notification;
using Unit.Shared.RequestFeatures;

namespace Unit.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IServiceManager _service;

        public NotificationController(IServiceManager service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllNotification(RequestParameters parameters, [FromHeader(Name = "Authorization")] string token)
        {
            var listNotificationDtos = await _service.NotificationService.GetAllNotificationOfUser(parameters, token);

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(listNotificationDtos.metaData));

            return Ok(listNotificationDtos);
        }

        [HttpDelete]
        [Authorize]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> DeleteNotification(NotificationDtoForDelete notificationDtoForDelete, [FromHeader(Name = "Authorization")] string token)
        {
            await _service.NotificationService.DeleteNotificationById(token, notificationDtoForDelete.CreatedAt);

            return Ok();
        }
    }
}
