using Amazon.DynamoDBv2.DataModel;
using Unit.Repository.Contracts;

namespace Unit.Repository
{
    public sealed class RepositoryManager : IRepositoryManager
    {
        public readonly Lazy<IUserRepository> _userRepository;

        public RepositoryManager(IDynamoDBContext dynamoDBContext)
        {
            _userRepository = new Lazy<IUserRepository>(() => new UserRepository(dynamoDBContext));
        }

        public IUserRepository User => _userRepository.Value;
    }
}
