using Amazon.CognitoIdentityProvider;
using AutoMapper;
using Microsoft.Extensions.Options;
using Unit.Entities.ConfigurationModels;
using Unit.Repository.Contracts;
using Unit.Service.Contracts;
using Unit.Shared.DataTransferObjects;

namespace Unit.Service
{
    public sealed class ServiceManager : IServiceManager
    {
        private readonly Lazy<IUserService> _userService;
        private readonly Lazy<IAuthenticationService> _authenticationService;

        public ServiceManager(IRepositoryManager repositoryManager, ILoggerManager logger, IAmazonCognitoIdentityProvider cognitoProvider, IOptions<AWSConfiguration> configuration, IMapper mapper, IDataShaper<UserDto> userShaper)
        {
            _userService = new Lazy<IUserService>(() => new UserService(repositoryManager, logger, mapper, userShaper));
            _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(repositoryManager, logger, mapper, cognitoProvider, configuration));
        }

        public IUserService UserService => _userService.Value;

        public IAuthenticationService AuthenticationService => _authenticationService.Value;
    }
}
