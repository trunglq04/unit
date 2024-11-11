using Amazon.CognitoIdentityProvider;
using Amazon.DynamoDBv2.DataModel;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Unit.Entities.ConfigurationModels;
using Unit.Repository.Contracts;
using Unit.Service.Contracts;

namespace Unit.Service
{
    public sealed class ServiceManager : IServiceManager
    {
        private readonly Lazy<IUserService> _userService;
        private readonly Lazy<IAuthenticationService> _authenticationService;

        public ServiceManager(IRepositoryManager repositoryManager, ILoggerManager logger, IAmazonCognitoIdentityProvider cognitoProvider, IOptions<AWSConfiguration> configuration, IMapper mapper)
        {
            _userService = new Lazy<IUserService>(() => new UserService(repositoryManager, logger));
            _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(repositoryManager, logger, cognitoProvider, configuration, mapper));
        }

        public IUserService UserService => _userService.Value;

        public IAuthenticationService AuthenticationService => _authenticationService.Value;
    }
}
