﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Unit.API.ActionFilter;
using Unit.Entities.ErrorModel;
using Unit.Service.Contracts;
using Unit.Shared.DataTransferObjects.Comment;
using Unit.Shared.DataTransferObjects.Reply;
using Unit.Shared.RequestFeatures;

namespace Unit.API.Controllers
{
    [Route("api/post/{postId}")]
    [ApiController]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public class CommentController : ControllerBase
    {
        private readonly IServiceManager _service;

        public CommentController(IServiceManager service)
        {
            _service = service;
        }

        [HttpGet("comments")]
        public async Task<IActionResult> GetCommentsByPostId([FromQuery] CommentParameters? commentParameters, string postId)
        {
            var comments = await _service.CommentService.GetCommentsByPostIdAsync(commentParameters, postId);

            if (comments.commentsDto != null)
            {
                Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(comments.metaData));
                return Ok(comments.commentsDto);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("comment")]
        [Authorize]
        public async Task<IActionResult> CreateComment(
            [FromHeader(Name = "Authorization")] string token,
            [FromBody] CreateCommentDto comment,
            string postId)
        {
            comment.PostId = postId;
            await _service.CommentService.CreateCommentAsync(comment, token);

            return Ok();
        }

        [HttpPut("comment/{commentId}")]
        [Authorize]
        public async Task<IActionResult> UpdateComment(
            [FromHeader(Name = "Authorization")] string token,
            [FromBody] UpdateCommentDto comment,
            string postId,
            string commentId)
        {
            comment.CommentId = commentId;
            comment.PostId = postId;
            await _service.CommentService.UpdateCommentAsync(comment, token);

            return Ok();
        }

        [HttpDelete("comment/{commentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(
            [FromHeader(Name = "Authorization")] string token,
            string postId,
            string commentId, [FromQuery] string postAuthorId)
        {
            if (string.IsNullOrWhiteSpace(postAuthorId))
                return new BadRequestObjectResult(new ErrorDetails()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Post aurthor is null"
                });
            var comment = new CommentDto
            {
                CommentId = commentId,
                PostId = postId,
                Content = string.Empty,
            };

            await _service.CommentService.DeleteCommentAsync(comment, token, postAuthorId);

            return Ok();
        }

        [HttpGet("comment/{commentId}")]
        public async Task<IActionResult> GetCommentById(string postId, string commentId)
        {
            var comment = await _service.CommentService.GetCommentByIdAsync(postId, commentId);

            if (comment != null)
            {
                return Ok(comment);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPut("comment/{commentId}/like")]
        [Authorize]
        public async Task<IActionResult> LikeComment(
            [FromHeader(Name = "Authorization")] string token,
            string postId,
            string commentId)
        {
            await _service.CommentService.LikeCommentAsync(postId, commentId, token);

            return Ok();
        }

        [HttpPost("comment/{parentCommentId}/reply")]
        [Authorize]
        public async Task<IActionResult> ReplyComment(
            [FromHeader(Name = "Authorization")] string token,
            [FromBody] CreateReplyDto reply,
            string postId,
            string parentCommentId)
        {
            await _service.CommentService.CreateReplyAsync(postId, parentCommentId, reply, token);

            return Ok();
        }

        [HttpPut("comment/{parentCommentId}/reply/{replyId}")]
        [Authorize]
        public async Task<IActionResult> UpdateReply(
            [FromHeader(Name = "Authorization")] string token,
            [FromBody] UpdateReplyDto reply,
            string postId,
            string parentCommentId,
            string replyId)
        {
            reply.ReplyId = replyId;
            await _service.CommentService.UpdateReplyAsync(postId, parentCommentId, reply, token);

            return Ok();
        }

        [HttpDelete("comment/{parentCommentId}/reply/{replyId}")]
        [Authorize]
        public async Task<IActionResult> DeleteReply(
            [FromHeader(Name = "Authorization")] string token,
            string postId,
            string parentCommentId,
            string replyId)
        {

            await _service.CommentService.DeleteReplyAsync(postId, parentCommentId, replyId, token);

            return Ok();
        }

        [HttpGet("comment/{parentCommentId}/replies")]
        public async Task<IActionResult> GetRepliesByCommentId(
            string postId,
            string parentCommentId)
        {
            var replies = await _service.CommentService.GetRepliesByCommentIdAsync(postId, parentCommentId);
            return Ok(replies);
        }
    }
}
