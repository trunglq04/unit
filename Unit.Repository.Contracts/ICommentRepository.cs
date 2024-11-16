using Unit.Entities.Models;
using Unit.Shared.RequestFeatures;

namespace Unit.Repository.Contracts
{
    public interface ICommentRepository
    {
        Task<PagedList<Comment>> GetCommentsByPostId(CommentParameters parameters,string postId);
        Task CreateCommentAsync(Comment comment);
        Task UpdateCommentAsync(Comment comment);
        Task DeleteCommentAsync(Comment comment);

        Task<Comment> GetCommentByKey(string postId, string commentId);

        //Task DeleteReplyAsync(Comment comment, string replyId);

        Task LikeCommentAsync(Comment comment, string likeAuthorId);
    }
}
