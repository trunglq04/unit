using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Unit.Repository.Contracts;

namespace Unit.Repository
{
    public sealed class RepositoryManager : IRepositoryManager
    {
        public readonly Lazy<IUserRepository> _userRepository;
        public readonly Lazy<ICommentRepository> _commentRepository;
        public readonly Lazy<INestedReplyRepository> _nestedReplyRepository;
        public readonly Lazy<IPostRepository> _postRepository;
        public readonly Lazy<IPostLikeListsRepository> _postLikeListsRepository;


        public RepositoryManager(IDynamoDBContext dynamoDBContext, IAmazonDynamoDB dynamoDbClient)
        {
            _userRepository = new Lazy<IUserRepository>(() => new UserRepository(dynamoDBContext, dynamoDbClient));
            _postRepository = new Lazy<IPostRepository>(() => new PostRepository(dynamoDBContext, dynamoDbClient));
            _commentRepository = new Lazy<ICommentRepository>(() => new CommentRepository(dynamoDBContext, dynamoDbClient));
            _nestedReplyRepository = new Lazy<INestedReplyRepository>(() => new NestedReplyRepository(dynamoDBContext, dynamoDbClient));
            _postLikeListsRepository = new Lazy<IPostLikeListsRepository>(() => new PostLikeListsRepository(dynamoDBContext, dynamoDbClient));
        }

        public IUserRepository User => _userRepository.Value;
        public IPostRepository Post => _postRepository.Value;
        public ICommentRepository Comment => _commentRepository.Value;

        public IPostLikeListsRepository PostLikeLists => _postLikeListsRepository.Value;
        public INestedReplyRepository NestedReply => _nestedReplyRepository.Value;
    }
}
