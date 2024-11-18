using System.Dynamic;
using Unit.Shared.DataTransferObjects.Comment;
using Unit.Shared.DataTransferObjects.Reply;
using Unit.Shared.RequestFeatures;

namespace Unit.Service.Contracts
{
    public interface ICommentService 
    {
        Task<(IEnumerable<ExpandoObject> commentsDto, MetaData metaData)> GetCommentsByPostIdAsync(CommentParameters parameters, string postId);

        Task CreateCommentAsync(CreateCommentDto comment, string token);

        Task UpdateCommentAsync(UpdateCommentDto comment, string token);

        Task DeleteCommentAsync(CommentDto comment, string token);

        Task<ExpandoObject> GetCommentByIdAsync(string postId, string commentId);

        Task LikeCommentAsync(string postId, string commentId, string token);

        Task CreateReplyAsync(string postId, string parentCommentId, CreateReplyDto replyDto, string token);
        Task UpdateReplyAsync(string postId, string parentCommentId, UpdateReplyDto replyDto, string token);
        Task DeleteReplyAsync(string postId, string parentCommentId, string replyId, string token);
        Task<IEnumerable<ExpandoObject>> GetRepliesByCommentIdAsync(string postId, string parentCommentId);
    }
}
