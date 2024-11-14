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

            filterExpression.Append("is_hidden = :isHidden");

            var expressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":isHidden", new AttributeValue { N = "0" } },
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


            var posts = await FindByConditionAsync(
                request,
                filterExpression,
                expressionAttributeValues
                );
            var listPosts = posts.listEntity.Sort(request.OrderBy).ToList();


            return new PagedList<Post>(listPosts, posts.pageKey, request.Size);
        }

        public Task<PagedList<Post>> GetPostsByUserId(PostParameters request, string userId, string postId)
        {
            //var

            //var filterExpression = new StringBuilder();

            //filterExpression.Append("is_hidden = :isHidden");
            throw new NotImplementedException();
        }

        public async Task UpdateUserAsync(Post post)
         => await UpdateUserAsync(post);
    }
}
