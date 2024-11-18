using Amazon.CognitoIdentityProvider.Model;
using AutoMapper;
using System.Dynamic;
using System.Xml.Linq;
using Unit.Entities.Exceptions;
using Unit.Entities.Models;
using Unit.Repository.Contracts;
using Unit.Service.Contracts;
using Unit.Service.Helper;
using Unit.Shared.DataTransferObjects.Comment;
using Unit.Shared.DataTransferObjects.Reply;
using Unit.Shared.RequestFeatures;

namespace Unit.Service
{
    public class CommentService : ICommentService
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;
        private readonly IDataShaper<ResponseCommentDto> _commentShaper;
        private readonly IDataShaper<ResponseReplyDto> _replyShaper;

        public CommentService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, IDataShaper<ResponseCommentDto> commentShaper, IDataShaper<ResponseReplyDto> replyShaper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _commentShaper = commentShaper;
            _replyShaper = replyShaper;
        }

        public async Task<(IEnumerable<ExpandoObject> commentsDto, MetaData metaData)> GetCommentsByPostIdAsync(
            CommentParameters parameters, 
            string postId)
        {
            var comments = await _repository.Comment.GetCommentsByPostId(parameters, postId);
            
            return await CommentEntityToDto(comments, parameters);
        }

        public async Task CreateCommentAsync(
            CreateCommentDto comment, 
            string token)
        {
            var userId = JwtHelper.GetPayloadData(token, "username");

            var commentEntity = _mapper.Map<Comment>(comment);

            commentEntity.AuthorId = userId;

            await _repository.Comment.CreateCommentAsync(commentEntity);
        }

        public async Task UpdateCommentAsync(
            UpdateCommentDto comment, 
            string token)
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

            // Updated comment
            commentEntity.AuthorId = userId;
            commentEntity.CreatedAt = checkedCmt.CreatedAt;
            commentEntity.Metadata.IsEdited = true;

            await _repository.Comment.UpdateCommentAsync(commentEntity);
        }

        public async Task DeleteCommentAsync(
            CommentDto comment, 
            string token)
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
            //
        }

        public async Task<ExpandoObject> GetCommentByIdAsync(string postId, string commentId)
        {
            var comment = await _repository.Comment.GetCommentByKey(postId, commentId);

            var commentDto = _mapper.Map<ResponseCommentDto>(comment);

            var shapedData = _commentShaper.ShapeData(commentDto, null);

            return shapedData;
        }

        private async Task<(IEnumerable<ExpandoObject> commentsDto, MetaData metaData)> CommentEntityToDto(
            PagedList<Comment> comments,
            CommentParameters parameters)
        {
            var commentDtos = _mapper.Map<IEnumerable<ResponseCommentDto>>(comments);

            foreach (var commentDto in commentDtos)
            {   
                var author = await _repository.User.GetUserAsync(commentDto.AuthorId);
                commentDto.LikeCount = commentDto.Metadata?.Likes?.Count;
                commentDto.AuthorUserName = author.UserName;
                commentDto.AuthorProfilePicture = author.ProfilePicture;
            }

            var shapedComments = _commentShaper.ShapeData(commentDtos, parameters.Fields);

            return (commentsDto: shapedComments, metaData: comments.MetaData);
        }

        public async Task LikeCommentAsync(
            string postId, 
            string commentId, 
            string token)
        {
            var checkedCmt = await _repository.Comment.GetCommentByKey(postId, commentId);

            if (checkedCmt == null)
            {
                throw new NotFoundException("Invalid action! Comment not found!");
            }

            var likeAuthorId = JwtHelper.GetPayloadData(token, "username");
            var user = await _repository.User.GetUserAsync(likeAuthorId);

            await _repository.Comment.LikeCommentAsync(checkedCmt, likeAuthorId!);
        }

        public async Task CreateReplyAsync(
            string postId, 
            string parentCommentId, 
            CreateReplyDto replyDto, 
            string token)
        {
            var nestedReply = await _repository.NestedReply.GetNestedReplyAsync(postId, parentCommentId);

            var userId = JwtHelper.GetPayloadData(token, "username");

            if (nestedReply == null)
            {
                var newReply = _mapper.Map<Reply>(replyDto);
                newReply.AuthorId = userId!;
                nestedReply = new NestedReply
                {
                    PostId = postId,
                    ParentCommentId = parentCommentId,
                    Replies = new List<Reply> { newReply }
                };
                await _repository.NestedReply.CreateNestedReplyAsync(nestedReply);
            } 
            else
            {
                var newReply = _mapper.Map<Reply>(replyDto);
                newReply.AuthorId = userId!;

                await _repository.NestedReply.CreateReplyAsync(nestedReply, newReply);
            }
        }

        public async Task UpdateReplyAsync(
            string postId, 
            string parentCommentId, 
            UpdateReplyDto replyDto, 
            string token)
        {
            var userId = JwtHelper.GetPayloadData(token, "username");
            replyDto.AuthorId = userId!;
            var updatedReply = _mapper.Map<Reply>(replyDto);

            var nestedReply = await _repository.NestedReply.GetNestedReplyAsync(postId, parentCommentId);

            await _repository.NestedReply.UpdateReplyAsync(nestedReply, updatedReply);
        }

        public async Task DeleteReplyAsync(
            string postId, 
            string commentId, 
            string replyId, 
            string token)
        {
            var userId = JwtHelper.GetPayloadData(token, "username");

            var nestedReply = await _repository.NestedReply.GetNestedReplyAsync(postId, commentId);

            if (nestedReply == null)
            {
                throw new NotFoundException("Invalid action! Reply not found!");
            }

            var deletedReply = new Reply { ReplyId = replyId, AuthorId = userId!, Content="" };

            await _repository.NestedReply.DeleteReplyAsync(nestedReply, deletedReply);
        }

        public async Task<IEnumerable<ExpandoObject>> GetRepliesByCommentIdAsync(
            string postId, 
            string parentCommentId)
        {
            var replies = await _repository.NestedReply.GetRepliesAsync(postId, parentCommentId);

            return await ReplyEntityToDto(replies);
        }

        private async Task<IEnumerable<ExpandoObject>> ReplyEntityToDto(
            IEnumerable<Reply> replies)
        {
            var repliesDto = _mapper.Map<IEnumerable<ResponseReplyDto>>(replies);

            foreach (var replyDto in repliesDto)
            {
                var author = await _repository.User.GetUserAsync(replyDto.AuthorId);
                replyDto.AuthorUserName = author.UserName;
                replyDto.AuthorProfilePicture = author.ProfilePicture;
            }

            var shapedReplies = _replyShaper.ShapeData(repliesDto, null);

            return shapedReplies;
        }
    }
}
