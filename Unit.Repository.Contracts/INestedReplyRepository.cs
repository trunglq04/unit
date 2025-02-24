using Unit.Entities.Models;
using Unit.Shared.RequestFeatures;

namespace Unit.Repository.Contracts
{
    public interface INestedReplyRepository 
    {
        Task<NestedReply> GetNestedReplyAsync(string postId, string parentCommentId);
        Task CreateNestedReplyAsync(NestedReply nestedReply);
        Task DeleteNestedReplyAsync(NestedReply nestedReply);
        Task UpdateNestedReplyAsync(NestedReply nestedReply);

        // REPLY
        Task<IEnumerable<Reply>?> GetRepliesAsync(string postId, string parentCommentId); 
        Task<Reply> GetReplyAsync(string postId, string parentCommentId, string replyId);
        Task CreateReplyAsync(NestedReply nestedReply, Reply reply);
        Task DeleteReplyAsync(NestedReply nestedReply, Reply deletedReply);
        Task UpdateReplyAsync(NestedReply nestedReply, Reply updatedReply);
    }
}
