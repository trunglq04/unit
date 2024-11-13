using Amazon.S3;
using Amazon.S3.Model;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Unit.Entities.ConfigurationModels;
using Unit.Entities.Models;
using Unit.Repository.Contracts;
using Unit.Service.Contracts;
using Unit.Shared.DataTransferObjects;

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
            var postEntity = _mapper.Map<Post>(post);
            postEntity.CreatedAt = DateTime.UtcNow;
            postEntity.LastModified = DateTime.UtcNow;
            postEntity.UserId = userId;
            if (mediaPath != null && mediaPath.Any()) postEntity.Media.AddRange(mediaPath);
            await _repository.Post.CreatePostAsync(postEntity);
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
