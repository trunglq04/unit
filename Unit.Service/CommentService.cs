using Amazon.CognitoIdentityProvider.Model;
using AutoMapper;
using System.Dynamic;
using Unit.Entities.Exceptions;
using Unit.Entities.Models;
using Unit.Repository.Contracts;
using Unit.Service.Contracts;
using Unit.Service.Helper;
using Unit.Shared.DataTransferObjects.Comment;
using Unit.Shared.RequestFeatures;

namespace Unit.Service
{
    public class CommentService : ICommentService
    {
        private readonly ILoggerManager _logger;

        private readonly IRepositoryManager _repository;

        private readonly IMapper _mapper;

        private readonly IDataShaper<CommentDto> _commentShaper;

        public CommentService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, IDataShaper<CommentDto> commentShaper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _commentShaper = commentShaper;
        }

        public async Task<(IEnumerable<ExpandoObject> commentsDto, MetaData metaData)> GetCommentsByPostIdAsync(CommentParameters parameters, string postId)
        {
            var comments = await _repository.Comment.GetCommentsByPostId(parameters, postId);

            var shapedData = await EntityToDto(comments, parameters);

            return shapedData;
        }

        public async Task CreateCommentAsync(CommentDto comment, string token)
        {
            var userId = JwtHelper.GetPayloadData(token, "username");

            var commentEntity = _mapper.Map<Comment>(comment);
            commentEntity.AuthorId = userId;

            await _repository.Comment.CreateCommentAsync(commentEntity);
        }

        public async Task UpdateCommentAsync(CommentDto comment, string token)
        {
            var userId = JwtHelper.GetPayloadData(token, "username");

            var checkedCmt = await _repository.Comment.GetCommentByKey(comment.PostId, comment.CommentId);

            // Check if comment exists
            if (checkedCmt == null)
            {
                throw new NotFoundException("Post Id or Comment Id not found!");
            }

            // Check if user is the author of the comment
            if (checkedCmt.AuthorId != userId)
            {
                throw new ForbiddenException("You are not allowed to update this comment!");
            }

            var commentEntity = _mapper.Map<Comment>(comment);
            commentEntity.AuthorId = userId;

            await _repository.Comment.UpdateCommentAsync(commentEntity);
        }

        public async Task DeleteCommentAsync(CommentDto comment, string token)
        {
            var userId = JwtHelper.GetPayloadData(token, "username");

            var checkedCmt = await _repository.Comment.GetCommentByKey(comment.PostId, comment.CommentId);

            // Check if comment exists
            if (checkedCmt == null)
            {
                throw new NotFoundException("Post Id or Comment Id not found!");
            }

            // Check if user is the author of the comment
            if (checkedCmt.AuthorId != userId)
            {
                throw new ForbiddenException("You are not allowed to update this comment!");
            }

            var commentEntity = _mapper.Map<Comment>(comment);
            commentEntity.AuthorId = userId;

            await _repository.Comment.DeleteCommentAsync(commentEntity);
        }

        private async Task<(IEnumerable<ExpandoObject> commentsDto, MetaData metadata)> EntityToDto(PagedList<Comment> comments, CommentParameters parameters)
        {
            var commentDto = _mapper.Map<IEnumerable<CommentDto>>(comments);

            var shapedComments = _commentShaper.ShapeData(commentDto, parameters.Fields);

            return (commentsDto: shapedComments, comments.MetaData);
        }
    }
}
