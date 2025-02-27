﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Unit.API.ActionFilter;
using Unit.Service.Contracts;
using Unit.Shared.DataTransferObjects.Notification;
using Unit.Shared.RequestFeatures;

namespace Unit.API.Controllers
{
    [Route("api/notification")]
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
        public async Task<IActionResult> GetAllNotification([FromQuery] NotificationParameters parameters, [FromHeader(Name = "Authorization")] string token)
        {
            var listNotificationDtos = await _service.NotificationService.GetAllNotificationOfUser(parameters, token);

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(listNotificationDtos.metaData));

            return Ok(listNotificationDtos.notificationDtos);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateNotification([FromBody] NotificationDtoForUpdate notificationDtoForUpdate, [FromHeader(Name = "Authorization")] string token)
        {
            await _service.NotificationService.UpdateNotificationById(token, notificationDtoForUpdate.CreatedAt);
           
           return Ok();
        }

        [HttpDelete]
        [Authorize]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> DeleteNotification([FromBody] NotificationDtoForDelete notificationDtoForDelete, [FromHeader(Name = "Authorization")] string token)

        {
            await _service.NotificationService.DeleteNotificationById(token, notificationDtoForDelete.CreatedAt);

            return Ok();
        }
    }
}
