using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Unit.Repository.Contracts;

namespace Unit.Repository
{
    public sealed class RepositoryManager : IRepositoryManager
    {
        public readonly Lazy<IUserRepository> _userRepository;
        public readonly Lazy<IPostRepository> _postRepository;

        public RepositoryManager(IDynamoDBContext dynamoDBContext, IAmazonDynamoDB dynamoDbClient)
        {
            _userRepository = new Lazy<IUserRepository>(() => new UserRepository(dynamoDBContext, dynamoDbClient));
            _postRepository = new Lazy<IPostRepository>(() => new PostRepository(dynamoDBContext, dynamoDbClient));
        }

        public IUserRepository User => _userRepository.Value;
        public IPostRepository Post => _postRepository.Value;
    }
}
