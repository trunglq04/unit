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
        public CommentRepository(IDynamoDBContext dynamoDbContext, IAmazonDynamoDB dynamoDbClient) :
            base(dynamoDbContext, dynamoDbClient) { }

        public async Task<PagedList<Comment>> GetCommentsByPostId(CommentParameters parameters, string postId)
        {
            var filterExpression = new StringBuilder("post_id = :postId");
            var expressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":postId", new AttributeValue { S = postId } }
            };

            var comments = await FindByConditionAsync(
                parameters,
                filterExpression,
                expressionAttributeValues
            );

            var listComments = comments.listEntity.Sort(parameters.OrderBy).ToList();

            return new PagedList<Comment>(listComments, comments.pageKey, parameters.Size);
        }

        public async Task CreateCommentAsync(Comment comment)
            => await CreateAsync(comment);

        public async Task UpdateCommentAsync(Comment comment)
            => await UpdateAsync(comment);
        
        public async Task DeleteCommentAsync(Comment comment)
            => await DeleteAsync(comment.PostId, comment.CommentId);

        public async Task<Comment?> GetCommentByKey(string postId, string commentId)
            => await FindByIdAsync(postId, commentId);
        
        public async Task LikeCommentAsync(Comment comment, string likeAuthorId)
        {
            /* Check if the author already liked the comment then unlike else like the comment 
            */
            var likeAuthorList = comment.Metadata.Likes;

            if (likeAuthorList.Contains(likeAuthorId))
            {
                likeAuthorList.Remove(likeAuthorId);
            }   
            else
            {
                likeAuthorList.Add(likeAuthorId);
            }

            comment.Metadata.Likes = likeAuthorList;

            await UpdateAsync(comment);
        }   
    }
}