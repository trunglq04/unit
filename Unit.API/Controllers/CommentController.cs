using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unit.API.ActionFilter;
using Unit.Service.Contracts;
using Unit.Shared.DataTransferObjects.Comment;
using Unit.Shared.RequestFeatures;

namespace Unit.API.Controllers
{
    [Route("api/post/{postId}/comment")]
    [ApiController]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public class CommentController : ControllerBase
    {
        private readonly IServiceManager _service;

        public CommentController(IServiceManager service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetCommentsByPostId([FromQuery] CommentParameters commentParameters, string postId)
        {
            var comments = await _service.CommentService.GetCommentsByPostIdAsync(commentParameters, postId);

            if (comments.commentsDto != null)
            {
                return Ok(comments);
            } 
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateComment(
            [FromHeader(Name = "Authorization")] string token,
            [FromBody] CommentDto comment,
            string postId)
        {
            comment.PostId = postId;
            await _service.CommentService.CreateCommentAsync(comment, token);

            return Ok();
        }

        [HttpPut("{commentId}")]
        [Authorize]
        public async Task<IActionResult> UpdateComment(
            [FromHeader(Name = "Authorization")] string token, 
            [FromBody] CommentDto comment, 
            string postId,
            string commentId)
        {
            comment.CommentId = commentId;
            comment.PostId = postId;
            await _service.CommentService.UpdateCommentAsync(comment, token);

            return Ok();
        }

        [HttpDelete("{commentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(
            [FromHeader(Name = "Authorization")] string token,
            string postId,
            string commentId)
        {
            var comment = new CommentDto 
            { 
                CommentId = commentId,
                PostId = postId,
                Content = string.Empty,
            };

            await _service.CommentService.DeleteCommentAsync(comment, token);

            return Ok();
        }


    }
}
