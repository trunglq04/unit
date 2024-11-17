using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using System.Text;
using Unit.Entities.Models;
using Unit.Repository.Contracts;
using Unit.Shared.RequestFeatures;

namespace Unit.Repository
{
    public class PostLikeListsRepository : RepositoryBase<PostLikeList>, IPostLikeListsRepository
    {
        public PostLikeListsRepository(IDynamoDBContext dynamoDbContext, IAmazonDynamoDB dynamoDbClient) : base(dynamoDbContext, dynamoDbClient)
        {
        }



        public async Task CreatePostLikeListAsync(PostLikeList postLikeList)
            => await CreateAsync(postLikeList);

        public async Task<PagedList<PostLikeList>> GetPostLikedListsAsync(PostParameters request)
        {
            var keyCondition = new StringBuilder();
            keyCondition.Append("post_id = :postId");


            var expressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":postId", new AttributeValue { S = request.PostId } },
            };


            var postLikeList = await FindByConditionAsync(
                request,
                keyCondition,
                expressionAttributeValues
                );

            return new PagedList<PostLikeList>(postLikeList.listEntity.ToList(), postLikeList.pageKey, request.Size);
        }

        public async Task<bool> IsLikedPost(string postId, string userId)
            => (await FindByIdAsync(postId, userId) != null);

        public async Task RemovePostLikeListAsync(PostLikeList postLikeList)
            => await DeleteAsync(postLikeList.PostId, postLikeList.UserId);
    }
}
