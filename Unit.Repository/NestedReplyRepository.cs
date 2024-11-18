using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Unit.Entities.Models;
using Unit.Repository.Contracts;

namespace Unit.Repository
{
    public class NestedReplyRepository : RepositoryBase<NestedReply>, INestedReplyRepository
    {
        public NestedReplyRepository(IDynamoDBContext dynamoDbContext, IAmazonDynamoDB dynamoDbClient) : base(dynamoDbContext, dynamoDbClient)
        {
        }
        public async Task<NestedReply?> GetNestedReplyAsync(string postId, string parentCommentId)
            => await FindByIdAsync(partitionKey: postId, sortKey: parentCommentId);

        public async Task CreateNestedReplyAsync(NestedReply nestedReply)
            => await CreateAsync(nestedReply);

        public async Task DeleteNestedReplyAsync(NestedReply nestedReply)
            => await DeleteAsync(nestedReply.PostId, nestedReply.ParentCommentId);

        public async Task DeleteReplyAsync(NestedReply nestedReply, string replyId)
        {
            var reply = nestedReply.Replies.Find(r => r.ReplyId == replyId);
            if (reply != null) 
            { 
                nestedReply.Replies.Remove(reply);
                await UpdateAsync(nestedReply);
            }
        }
       
        public async Task UpdateNestedReplyAsync(NestedReply nestedReply)
            => await UpdateAsync(nestedReply);

        public async Task CreateReplyAsync(NestedReply nestedReply, Reply reply)
        {
            if (nestedReply.Replies is null)
            {
                nestedReply.Replies = new List<Reply> { reply };
            }
            else
            {
                nestedReply.Replies.Add(reply);
            }
            
            await UpdateNestedReplyAsync(nestedReply);
        }

        public async Task DeleteReplyAsync(NestedReply nestedReply, Reply deletedReply)
        {
            var replies = nestedReply.Replies;

            foreach (var r in replies)
            {
                if (r.ReplyId == deletedReply.ReplyId && r.AuthorId == deletedReply.AuthorId)
                {
                    replies.Remove(r);
                    nestedReply.Replies = replies;

                    await UpdateNestedReplyAsync(nestedReply);
                    break;
                }
            }
        }

        public async Task UpdateReplyAsync(NestedReply nestedReply, Reply updatedReply)
        {
            var replies = nestedReply.Replies;

            foreach (var r in replies)
            {
                if (r.ReplyId == updatedReply.ReplyId && r.AuthorId == updatedReply.AuthorId)
                {
                    r.Content = updatedReply.Content;
                    nestedReply.Replies = replies;

                    await UpdateNestedReplyAsync(nestedReply);
                    break;
                }
            }

            
        }

        public async Task<IEnumerable<Reply>?> GetRepliesAsync(string postId, string parentCommentId)
        {
            var nestedReply = await GetNestedReplyAsync(postId, parentCommentId);
            return nestedReply?.Replies;
        }

        public async Task<Reply?> GetReplyAsync(string postId, string parentCommentId, string replyId)
        {
            var replies = await GetRepliesAsync(postId, parentCommentId);

            if (replies != null)
            {
                foreach (var r in replies)
                {
                    if (r.ReplyId == replyId)
                    {
                        return r;
                    }
                }
            }
            return null;
        }
    }
}
