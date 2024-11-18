using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;
using System.Text.Json;
using Unit.API.ActionFilter;
using Unit.Service.Contracts;
using Unit.Service.Helper;
using Unit.Shared.DataTransferObjects.Post;
using Unit.Shared.RequestFeatures;

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
        public async Task<IActionResult> CreatePost(
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
                    var fileExtension = Path.GetExtension(imageFile.FileName);

                    using (var memoryStream = new MemoryStream())
                    {
                        await imageFile.CopyToAsync(memoryStream);
                        memoryStream.Position = 0;

                        var path = await _service.PostService.UploadMediaPostAsync(userId!, memoryStream, fileExtension);
                        mediaPath.Add(path);
                    }
                }
            }

            await _service.PostService.CreatePost(post, userId!, mediaPath);

            return Ok();
        }
        //userId o day la id cua nguoi gui request
        //userId trong PostParameters la userId cua query
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPosts([FromHeader(Name = "Authorization")] string token, [FromQuery] PostParameters postParameters)
        {
            if (postParameters.LikeList != null && (bool)postParameters.LikeList && postParameters.PostId != null && !string.IsNullOrWhiteSpace(postParameters.PostId))
            {
                var listLikedPost = await _service.PostService.GetListLikedPost(postParameters);

                var listUserId = listLikedPost.postLikeLists.Select(p => p.UserId).ToList();

                var listUser = await _service.UserService.GetUsersByIdsAsync(new() { Fields = "UserId,UserName,ProfilePicture" }, token, listUserId);

                Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(listLikedPost.metaData));

                return Ok(listUser.users);
            }
            var postWithMetaData = await _service.PostService.GetPosts(postParameters, token);

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(postWithMetaData.metaData));

            return Ok(postWithMetaData.posts);
        }

        [HttpPost("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePost(
            [FromHeader(Name = "Authorization")] string token,
            [FromBody] PostDtoForUpdate request, string id)
        {
            await _service.PostService.UpdatePost(id, request, token);

            return Ok();
        }

    }
}
