using Amazon.CognitoIdentityProvider;
using Amazon.S3;
using AutoMapper;
using Microsoft.Extensions.Options;
using Unit.Entities.ConfigurationModels;
using Unit.Repository.Contracts;
using Unit.Service.Contracts;
using Unit.Shared.DataTransferObjects.Comment;
using Unit.Shared.DataTransferObjects.Post;
using Unit.Shared.DataTransferObjects.Reply;
using Unit.Shared.DataTransferObjects.User;

namespace Unit.Service
{
    public sealed class ServiceManager : IServiceManager
    {
        private readonly Lazy<IUserService> _userService;
        private readonly Lazy<IAuthenticationService> _authenticationService;
        private readonly Lazy<IPostService> _postService;
        private readonly Lazy<ICommentService> _commentService;

        public ServiceManager(
            IRepositoryManager repositoryManager,
            ILoggerManager logger,
            IAmazonCognitoIdentityProvider cognitoProvider,
            IOptions<AWSConfiguration> configuration,
            IMapper mapper,
            IDataShaper<UserDto> userShaper,
            IDataShaper<PostDto> postShaper,
            IDataShaper<ResponseCommentDto> cmtShaper,
            IDataShaper<ReplyDto> replyShaper,
            IAmazonS3 s3Client)
        {
            _userService = new Lazy<IUserService>(() => new UserService(repositoryManager, logger, mapper, userShaper, s3Client, configuration));
            _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(repositoryManager, logger, mapper, cognitoProvider, configuration));
            _postService = new Lazy<IPostService>(() => new PostService(repositoryManager, logger, mapper, postShaper, s3Client, configuration));
            _commentService = new Lazy<ICommentService>(() => new CommentService(repositoryManager, logger, mapper, cmtShaper, replyShaper));
        }

        public IUserService UserService => _userService.Value;

        public IAuthenticationService AuthenticationService => _authenticationService.Value;
        public IPostService PostService => _postService.Value;

        public ICommentService CommentService => _commentService.Value;
    }
}
