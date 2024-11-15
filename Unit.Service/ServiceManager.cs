using Amazon.CognitoIdentityProvider;
using AutoMapper;
using Microsoft.Extensions.Options;
using Unit.Entities.ConfigurationModels;
using Unit.Entities.Models;
using Unit.Repository.Contracts;
using Unit.Service.Contracts;
using Unit.Shared.DataTransferObjects;
using Unit.Shared.DataTransferObjects.Comment;

namespace Unit.Service
{
    public sealed class ServiceManager : IServiceManager
    {
        private readonly Lazy<IUserService> _userService;
        private readonly Lazy<IAuthenticationService> _authenticationService;
        private readonly Lazy<ICommentService> _commentService;

        public ServiceManager(IRepositoryManager repositoryManager, ILoggerManager logger, IAmazonCognitoIdentityProvider cognitoProvider, IOptions<AWSConfiguration> configuration, IMapper mapper, IDataShaper<UserDto> userShaper, IDataShaper<CommentDto> cmtShaper)
        {
            _userService = new Lazy<IUserService>(() => new UserService(repositoryManager, logger, mapper, userShaper));
            _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(repositoryManager, logger, mapper, cognitoProvider, configuration));

            _commentService = new Lazy<ICommentService>(() => new CommentService(repositoryManager, logger, mapper, cmtShaper));
        }

        public IUserService UserService => _userService.Value;

        public IAuthenticationService AuthenticationService => _authenticationService.Value;

        public ICommentService CommentService => _commentService.Value;
    }
}
