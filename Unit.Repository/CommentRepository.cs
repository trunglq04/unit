using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using System.Text;
using Unit.Entities.Models;
using Unit.Repository.Contracts;
using Unit.Repository.Extensions;
using Unit.Shared.RequestFeatures;

namespace Unit.Repository
{
    public class CommentRepository : RepositoryBase<Comment>, ICommentRepository
    {
        public CommentRepository(IDynamoDBContext dynamoDbContext, IAmazonDynamoDB dynamoDbClient) : base(dynamoDbContext, dynamoDbClient) { }

        public async Task<PagedList<Comment>> GetCommentsByPostId(CommentParameters parameters, string postId)
        {
            var keyExpression = new StringBuilder("post_id = :postId");
            var expressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":postId", new AttributeValue { S = postId } }
            };

            var comments = await FindByConditionAsync(
                parameters,
                keyExpression,
                expressionAttributeValues
            );

            var listComments = comments.listEntity.Sort(parameters.OrderBy).ToList();

            return new PagedList<Comment>(listComments, comments.pageKey, parameters.Size);
        }

        public async Task CreateCommentAsync(Comment comment)
            => await CreateAsync(comment);

        public async Task UpdateCommentAsync(Comment comment)
        {
            await UpdateAsync(comment);
        }

        public async Task DeleteCommentAsync(Comment comment)
            => await DeleteAsync(comment.PostId, comment.CommentId);

        public async Task<Comment?> GetCommentByKey(string postId, string commentId)
        {
            return await FindByIdAsync(postId, commentId);
        }

        public async Task LikeCommentAsync(Comment comment, string likeAuthorId)
        {
            comment.Metadata.Likes.Add(likeAuthorId);

            await UpdateAsync(new Comment
            {
                PostId = comment.PostId,
                CommentId = comment.CommentId,
                Metadata = new Metadata
                {
                    Likes = comment.Metadata.Likes
                }
            });
        }


    }
}
