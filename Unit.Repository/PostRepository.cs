using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Unit.Entities.Models;
using Unit.Repository.Contracts;

namespace Unit.Repository
{
    public class PostRepository : RepositoryBase<Post>, IPostRepository
    {

        public PostRepository(IDynamoDBContext dynamoDbContext, IAmazonDynamoDB dynamoDbClient) : base(dynamoDbContext, dynamoDbClient) { }
        public async Task CreatePostAsync(Post post)
        => await CreateAsync(post);

        public async Task UpdateUserAsync(Post post)
         => await UpdateUserAsync(post);
    }
}
