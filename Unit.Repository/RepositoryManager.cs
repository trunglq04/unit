using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Unit.Repository.Contracts;

namespace Unit.Repository
{
    public sealed class RepositoryManager : IRepositoryManager
    {
        public readonly Lazy<IUserRepository> _userRepository;
        public readonly Lazy<ICommentRepository> _commentRepository;

        public RepositoryManager(IDynamoDBContext dynamoDBContext, IAmazonDynamoDB dynamoDbClient)
        {
            _userRepository = new Lazy<IUserRepository>(() => new UserRepository(dynamoDBContext, dynamoDbClient));
            _commentRepository = new Lazy<ICommentRepository>(() => new CommentRepository(dynamoDBContext, dynamoDbClient));
        }

        public IUserRepository User => _userRepository.Value;
        public ICommentRepository Comment => _commentRepository.Value;
    }
}
