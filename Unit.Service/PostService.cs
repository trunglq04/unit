using Amazon.S3;
using Amazon.S3.Model;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Unit.Entities.ConfigurationModels;
using Unit.Entities.Exceptions;
using Unit.Entities.Exceptions.Messages;
using Unit.Entities.Models;
using Unit.Repository.Contracts;
using Unit.Service.Contracts;
using Unit.Service.Helper;
using Unit.Shared.DataTransferObjects;
using Unit.Shared.RequestFeatures;

namespace Unit.Service
{
    public class PostService : IPostService
    {
        private readonly ILoggerManager _logger;

        private readonly IRepositoryManager _repository;

        private readonly IMapper _mapper;

        private readonly IDataShaper<PostDto> _postShaper;

        private readonly IAmazonS3 _s3Client;

        private readonly S3Configuration _s3Config;

        public PostService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, IDataShaper<PostDto> postShaper, IAmazonS3 s3Client, IOptions<AWSConfiguration> configuration)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _postShaper = postShaper;
            _s3Client = s3Client;
            _s3Config = configuration.Value.S3Bucket!;
        }

        public async Task CreatePost(PostDtoForCreation post, string userId, List<string>? mediaPath = null)
        {
            var user = await _repository.User.GetUserAsync(userId);
            if (!user.Active) throw new BadRequestException(UserExMsg.UserHasBeenDisable);

            var postEntity = _mapper.Map<Post>(post);

            postEntity.CreatedAt = DateTime.UtcNow;
            postEntity.LastModified = DateTime.UtcNow;
            postEntity.UserId = userId;
            postEntity.IsPrivate = user.Private;
            postEntity.UserName = user.UserName;
            postEntity.ProfilePicture = user.ProfilePicture;

            if (mediaPath != null && mediaPath.Any()) postEntity.Media.AddRange(mediaPath);

            await _repository.Post.CreatePostAsync(postEntity);
        }

        //userId o day la id cua nguoi gui request
        //userId trong PostParameters la userId cua query
        public async Task<(IEnumerable<PostDto> posts, MetaData metaData)> GetPosts(PostParameters request, string token)
        {
            var userId = JwtHelper.GetPayloadData(token, "username");

            if (!string.IsNullOrWhiteSpace(request.UserId) && !request.UserId.Equals(userId))
            {
                var user = await _repository.User.GetUserAsync(request.UserId!);

                if (user.Private && user.Followers.Any() && !user.Followers.Contains(userId!)) throw new BadRequestException(UserExMsg.DoNotHavePermissionToView);
                request.IsHidden = false;

                var postsEntity = await _repository.Post.GetPostsByUserId(request);

                var postsDto = _mapper.Map<List<PostDto>>(postsEntity);

                return (posts: postsDto, metaData: postsEntity.MetaData);
            }
            else if ((request.MyPost != null && (bool)request.MyPost) || (!string.IsNullOrWhiteSpace(request.UserId) && request.UserId.Equals(userId)))
            {
                request.UserId = userId;
                var postsEntity = await _repository.Post.GetPostsByUserId(request);

                var postsDto = _mapper.Map<List<PostDto>>(postsEntity);

                return (posts: postsDto, metaData: postsEntity.MetaData);
            }
            else
            {
                var user = await _repository.User.GetUserAsync(userId!);

                request.UserId = userId;
                var postsEntity = await _repository.Post.GetPosts(request, user.Following);

                var postsDto = _mapper.Map<List<PostDto>>(postsEntity);

                return (posts: postsDto, metaData: postsEntity.MetaData);
            }
        }

        public async Task<string> UploadMediaPostAsync(string userId, Stream fileStream, string fileExtension)
        {
            Guid myuuid = Guid.NewGuid();
            string myuuidAsString = myuuid.ToString();
            string fileName = $"posts/{userId}/{myuuidAsString}{fileExtension}";

            var uploadRequest = new PutObjectRequest
            {
                InputStream = fileStream,
                BucketName = _s3Config.BucketName,
                Key = fileName,
                ContentType = GetContentType(fileExtension),
            };

            uploadRequest.Headers.ContentDisposition = "inline";
            await _s3Client.PutObjectAsync(uploadRequest);

            return _s3Config.S3BucketPath + fileName;
        }


        private string GetContentType(string fileExtension)
        {
            return fileExtension.ToLower() switch
            {
                // Video content types
                ".mp4" => "video/mp4",
                ".mov" => "video/quicktime",
                ".avi" => "video/x-msvideo",
                ".wmv" => "video/x-ms-wmv",
                ".mkv" => "video/x-matroska",

                // Image content types
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                ".tiff" => "image/tiff",
                ".svg" => "image/svg+xml",

                _ => "application/octet-stream",
            };
        }
    }
}
