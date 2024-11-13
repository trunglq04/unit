using Amazon.CognitoIdentityProvider;
using Amazon.S3;
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
        private readonly Lazy<IPostService> _postService;

        public ServiceManager(IRepositoryManager repositoryManager, ILoggerManager logger, IAmazonCognitoIdentityProvider cognitoProvider, IOptions<AWSConfiguration> configuration, IMapper mapper, IDataShaper<UserDto> userShaper, IDataShaper<CommentDto> cmtShaper)
        public ServiceManager(IRepositoryManager repositoryManager, ILoggerManager logger, IAmazonCognitoIdentityProvider cognitoProvider, IOptions<AWSConfiguration> configuration, IMapper mapper, IDataShaper<UserDto> userShaper, IDataShaper<PostDto> postShaper, IAmazonS3 s3Client)
        {
            _userService = new Lazy<IUserService>(() => new UserService(repositoryManager, logger, mapper, userShaper, s3Client, configuration));
            _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(repositoryManager, logger, mapper, cognitoProvider, configuration));

            _commentService = new Lazy<ICommentService>(() => new CommentService(repositoryManager, logger, mapper, cmtShaper));
            _postService = new Lazy<IPostService>(() => new PostService(repositoryManager, logger, mapper, postShaper, s3Client, configuration));
        }

        public IUserService UserService => _userService.Value;

        public IAuthenticationService AuthenticationService => _authenticationService.Value;

        public ICommentService CommentService => _commentService.Value;

        public IPostService PostService => _postService.Value;

    }
}
