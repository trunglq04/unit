using Amazon.CognitoIdentityProvider.Model;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime.Internal;
using Unit.Repository.Contracts;
using Unit.Service.Contracts;

namespace Unit.Service
{
    public sealed class UserService : IUserService
    {
        private readonly ILoggerManager _logger;

        private readonly IRepositoryManager _repository;

        public UserService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
        }


    }
}
