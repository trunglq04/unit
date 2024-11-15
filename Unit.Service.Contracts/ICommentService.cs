using System.Dynamic;
using Unit.Shared.DataTransferObjects.Comment;
using Unit.Shared.RequestFeatures;

namespace Unit.Service.Contracts
{
    public interface ICommentService 
    {
        Task<(IEnumerable<ExpandoObject> commentsDto, MetaData metaData)> GetCommentsByPostIdAsync(CommentParameters parameters, string postId);

        Task CreateCommentAsync(CommentDto comment, string token);

        Task UpdateCommentAsync(CommentDto comment, string token);

        Task DeleteCommentAsync(CommentDto comment, string token);

    }
}
