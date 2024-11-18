using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Unit.Entities.Models;
using Unit.Repository.Contracts;
using Unit.Shared.RequestFeatures;
using Amazon.DynamoDBv2.Model;
using System.Text;
using Unit.Repository.Extensions;

namespace Unit.Repository
{
    public class PostRepository : RepositoryBase<Post>, IPostRepository
    {

        public PostRepository(IDynamoDBContext dynamoDbContext, IAmazonDynamoDB dynamoDbClient) : base(dynamoDbContext, dynamoDbClient) { }

        public async Task CreatePostAsync(Post post)
        => await CreateAsync(post);

        public Task<Post> GetPostByPostId(PostParameters request, string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedList<Post>> GetPosts(PostParameters request, List<string>? userFollowing = null)
        {
            var filterExpression = new StringBuilder();

            filterExpression.Append(" user_id = :userId OR is_hidden = :isHidden");

            var expressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":isHidden", new AttributeValue { N = "0" } },
                { ":userId", new AttributeValue { S = request.UserId } },

            };

            if (userFollowing != null && userFollowing.Any())
            {
                filterExpression.Append(" AND (is_private = :isPrivate AND (");

                for (int i = 0; i < userFollowing.Count; i++)
                {
                    var parameterName = $":userFollowing{i}";
                    if (i > 0)
                    {
                        filterExpression.Append(" OR ");
                    }
                    filterExpression.Append($"user_id = {parameterName}");
                    expressionAttributeValues[parameterName] = new AttributeValue { S = userFollowing[i] };
                }

                filterExpression.Append(") OR is_private = :isNotPrivate)");
                expressionAttributeValues[":isPrivate"] = new AttributeValue { N = "1" };
                expressionAttributeValues[":isNotPrivate"] = new AttributeValue { N = "0" };
            }
            else
            {
                filterExpression.Append(" AND is_private = :isNotPrivate");
                expressionAttributeValues[":isNotPrivate"] = new AttributeValue { N = "0" };
            }

            if (request.PostId != null && !string.IsNullOrWhiteSpace(request.PostId))
            {
                filterExpression.Append(" AND post_id = :postId");
                expressionAttributeValues[":postId"] = new AttributeValue() { S = request.PostId };
            }


            var posts = await FindByConditionAsync(
                request,
                filterExpression,
                expressionAttributeValues
                );
            var listPosts = posts.listEntity.Sort(request.OrderBy).ToList();


            return new PagedList<Post>(listPosts, posts.pageKey, request.Size);
        }

        //userId o day la id cua nguoi gui request
        //userId trong PostParameters la userId cua query
        public async Task<PagedList<Post>> GetPostsByUserId(PostParameters request)
        {

            var keyCondition = new StringBuilder();
            keyCondition.Append("user_id = :userId");

            var filterExpression = new StringBuilder();

            filterExpression.Append("is_hidden = :isHidden");

            var expressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":userId", new AttributeValue { S = request.UserId } },
                { ":isHidden", new AttributeValue { N = "0" } }
            };

            if (request.PostId != null && !string.IsNullOrWhiteSpace(request.PostId))
            {
                keyCondition.Append(" AND post_id = :postId");
                expressionAttributeValues[":postId"] = new AttributeValue() { S = request.PostId };
            }

            if (request.IsHidden != null && (bool)request.IsHidden)
            {
                expressionAttributeValues[":isHidden"] = new AttributeValue() { N = "1" };
            }
            if (filterExpression.Length == 0) filterExpression = null;
            var posts = await FindByConditionAsync(
                request,
                keyCondition,
                filterExpression,
                expressionAttributeValues
                );
            var listPosts = posts.listEntity.Sort(request.OrderBy).ToList();

            return new PagedList<Post>(listPosts, posts.pageKey, request.Size);
        }

        public async Task UpdatePostAsync(Post post)
         => await UpdateAsync(post);
    }
}
