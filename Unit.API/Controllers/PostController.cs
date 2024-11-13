using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unit.API.ActionFilter;
using Unit.Service.Contracts;
using Unit.Service.Helper;
using Unit.Shared.DataTransferObjects;

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

        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(FileValidationFilter))]
        public async Task<IActionResult> UpdateUser(
        [FromHeader(Name = "Authorization")] string token,
        [FromForm] PostDtoForCreation post,
        [FromForm] List<IFormFile>? media)
        {
            var userId = JwtHelper.GetPayloadData(token, "username");
            List<string> mediaPath = new List<string>();

            if (media != null)
            {
                foreach (var imageFile in media)
                {
                    // Lấy phần mở rộng tệp từ IFormFile
                    var fileExtension = Path.GetExtension(imageFile.FileName);

                    using (var memoryStream = new MemoryStream())
                    {
                        await imageFile.CopyToAsync(memoryStream);
                        memoryStream.Position = 0; // Đặt lại vị trí của stream về đầu

                        var path = await _service.PostService.UploadMediaPostAsync(userId!, memoryStream, fileExtension);
                        mediaPath.Add(path);
                    }
                }
            }

            await _service.PostService.CreatePost(post, userId!, mediaPath);

            return Ok();
        }

    }
}
