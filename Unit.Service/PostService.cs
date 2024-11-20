using Amazon.S3;
using Amazon.S3.Model;
using AutoMapper;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Unit.Entities.ConfigurationModels;
using Unit.Entities.Exceptions;
using Unit.Entities.Exceptions.Messages;
using Unit.Entities.Models;
using Unit.Repository.Contracts;
using Unit.Service.Contracts;
using Unit.Service.Helper;
using Unit.Shared.DataTransferObjects.Post;
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
            PagedList<Post> postsEntity;
            IEnumerable<PostDto> postDto;
            if (!string.IsNullOrWhiteSpace(request.UserId) && !request.UserId.Equals(userId))
            {
                var user = await _repository.User.GetUserAsync(request.UserId!);

                if (user.Private && user.Followers.Any() && !user.Followers.Contains(userId!)) throw new BadRequestException(UserExMsg.DoNotHavePermissionToView);
                request.IsHidden = false;
            }
            else if ((request.MyPost != null && (bool)request.MyPost) || (!string.IsNullOrWhiteSpace(request.UserId) && request.UserId.Equals(userId)))
            {
                request.UserId = userId;
            }
            else
            {
                var user = await _repository.User.GetUserAsync(userId!);

                request.UserId = userId;

                postsEntity = await _repository.Post.GetPosts(request, user.Following);
                postDto = await MapPostsWithLikeStatus(postsEntity, userId!);
                return (postDto, postsEntity.MetaData);

            }

            postsEntity = await _repository.Post.GetPostsByUserId(request);
            postDto = await MapPostsWithLikeStatus(postsEntity, userId!);
            return (postDto, postsEntity.MetaData);

        }
        private async Task<IEnumerable<PostDto>> MapPostsWithLikeStatus(PagedList<Post> postsEntity, string userId)
        {
            return await Task.WhenAll(
                _mapper.Map<List<PostDto>>(postsEntity)
                    .Select(async p =>
                    {
                        p.IsLiked = await _repository.PostLikeLists.IsLikedPost(p.PostId, userId);
                        return p;
                    })
            );
        }



        public async Task UpdatePost(string postId, PostDtoForUpdate post, string token, bool comment = false)
        {
            var userId = JwtHelper.GetPayloadData(token, "username");

            var postList = await _repository.Post.GetPostsByUserId(new()
            {
                PostId = postId,
                UserId = post.UserId
            });


            if (postList == null || !postList.Any())
                throw new NotFoundException(PostExMsg.PostNotFound);

            var postEntity = postList.FirstOrDefault()!;

            if (!post.UserId.Equals(userId))
            {
                var user = await _repository.User.GetUserAsync(post.UserId);

                if ((user.Private && !user.Followers.Contains(userId!)) || (post.Hidden != null && (bool)post.Hidden) || (post.Content != null && !string.IsNullOrWhiteSpace(post.Content)))
                    throw new BadRequestException(UserExMsg.DoNotHavePermissionToView);
            }

            if (userId!.Equals(post.UserId))
            {
                postEntity.IsHidden = post.Hidden ?? postEntity.IsHidden;
                if (post.Content != null && !string.IsNullOrWhiteSpace(post.Content))
                {
                    postEntity.Content = post.Content;
                    postEntity.LastModified = DateTime.UtcNow;
                }
            }

            if (post.Like != null)
            {
                var isLiked = await _repository.PostLikeLists.IsLikedPost(postId, userId).ConfigureAwait(false);
                if ((bool)post.Like && isLiked)
                {
                    throw new BadRequestException(UserExMsg.AlreadyLikedPost);
                }
                else if (!(bool)post.Like && !isLiked)
                {
                    throw new BadRequestException(UserExMsg.AlreadyUnLikedPost);
                }

                if ((bool)post.Like && !isLiked)
                {
                    await _repository.PostLikeLists.CreatePostLikeListAsync(
                    new()
                    {
                        PostId = postId,
                        UserId = userId
                    });
                    postEntity.LikeCount += 1;
                }
                else if (!(bool)post.Like && isLiked)
                {
                    await _repository.PostLikeLists.RemovePostLikeListAsync(
                        new()
                        {
                            PostId = postId,
                            UserId = userId
                        });

                    postEntity.LikeCount -= 1;
                }
            }

            if (comment) postEntity.CommentCount++;

            await _repository.Post.UpdatePostAsync(postEntity);

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

        public async Task<(IEnumerable<PostLikeList> postLikeLists, MetaData metaData)> GetListLikedPost(PostParameters request)
        {
            var list = await _repository.PostLikeLists.GetPostLikedListsAsync(request);

            if (list == null || !list.Any())
                throw new NotFoundException(PostExMsg.PostNotHaveAnyLike);

            return (list, list.MetaData);
        }
    }
}
