using Amazon.S3;
using Amazon.S3.Model;
using AutoMapper;
using Microsoft.Extensions.Options;
using System.Dynamic;
using Unit.Entities.ConfigurationModels;
using Unit.Entities.Models;
using Unit.Repository.Contracts;
using Unit.Service.Contracts;
using Unit.Service.Helper;
using Unit.Shared.DataTransferObjects.User;
using Unit.Shared.RequestFeatures;

namespace Unit.Service
{
    public sealed class UserService : IUserService
    {
        private readonly ILoggerManager _logger;

        private readonly IRepositoryManager _repository;

        private readonly IMapper _mapper;

        private readonly IDataShaper<UserDto> _userShaper;

        private readonly IAmazonS3 _s3Client;

        private readonly S3Configuration _s3Config;

        private readonly string _audience;

        public UserService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, IDataShaper<UserDto> userShaper, IAmazonS3 s3Client,
            IOptions<AWSConfiguration> configuration)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _userShaper = userShaper;
            _s3Client = s3Client;
            _s3Config = configuration.Value.S3Bucket!;
            _audience = configuration.Value.Audience!;
        }

        public async Task<ExpandoObject> GetUserByIdAsync(UserParameters parameters, string token, string? id = null)
        {
            var userId = JwtHelper.GetPayloadData(token, "username");
            var users = !string.IsNullOrWhiteSpace(id) ? await _repository.User.GetUserAsync(id!) : await _repository.User.GetUserAsync(userId!);
            var userDto = _mapper.Map<UserDto>(users);

            userDto.NumberOfFollowing = userDto.Following.Count;
            userDto.NumberOfFollwers = userDto.Followers.Count;

            if (!string.IsNullOrWhiteSpace(id) && !id.Equals(userId))
            {
                ConfigUserDto(ref userDto, userId);
            }
            else
            {
                userDto.isFollowed = null;
                foreach (var followRequest in userDto.FollowRequests)
                {
                    var userInFollowRequest = await _repository.User.GetUserAsync(followRequest.FollowerId);
                    if (userInFollowRequest != null)
                    {
                        followRequest.PictureProfile = userInFollowRequest.ProfilePicture;
                        followRequest.UserName = userInFollowRequest.UserName;
                    }
                }

            }

            var shapedData = _userShaper.ShapeData(userDto, parameters.Fields);

            return shapedData;
        }


        public async Task<(IEnumerable<ExpandoObject> users, MetaData metaData)> GetUsersAsync(
            UserParameters parameters,
            string token)
        {
            var userId = JwtHelper.GetPayloadData(token, "username");

            var usersWithMetaData = await _repository.User.GetUsersExceptAsync(parameters, userId!);

            return await MappingUserEntityToDto(usersWithMetaData, userId!, parameters.Fields);
        }

        public async Task<(IEnumerable<ExpandoObject> users, MetaData metaData)> GetUsersByIdsAsync(
            UserParameters parameters,
            string token,
            List<string> ids)
        {
            var userId = JwtHelper.GetPayloadData(token, "username");

            var usersWithMetaData = await _repository.User.GetUsersByIdsAsync(parameters, ids);

            return await MappingUserEntityToDto(usersWithMetaData, userId!, parameters.Fields);
        }

        public async Task UpdateUser(
            UserInfoDtoForUpdate userDtoForUpdate,
            string id,
            string? imagePath = null)
        {
            var userEntity = await _repository.User.GetUserAsync(id);

            await UpdatePostOfUser(userDtoForUpdate, id, imagePath);

            await UpdateFollowingOfUser(userDtoForUpdate, id, userEntity);

            await UpdateFollowerOfUser(userDtoForUpdate, id, userEntity);

            _mapper.Map(userDtoForUpdate, userEntity);

            if (!string.IsNullOrWhiteSpace(imagePath))
                userEntity.ProfilePicture = imagePath;

            userEntity.LastModified = DateTime.UtcNow;
            if (userEntity.Followers != null && userEntity.Followers.Count == 0) userEntity.Followers = null;
            if (userEntity.Following != null && userEntity.Following.Count == 0) userEntity.Following = null;
            await _repository.User.UpdateUserAsync(userEntity);

        }

        private async Task UpdatePostOfUser(UserInfoDtoForUpdate userDtoForUpdate, string id, string? imagePath)
        {
            if (userDtoForUpdate.Private != null || !string.IsNullOrWhiteSpace(userDtoForUpdate.UserName) || !string.IsNullOrWhiteSpace(imagePath))
            {
                var listPost = await _repository.Post.GetPostsByUserId(new()
                {
                    UserId = id
                });

                foreach (var post in listPost)
                {
                    post.IsPrivate = userDtoForUpdate.Private ?? post.IsPrivate;
                    post.UserName = userDtoForUpdate.UserName ?? post.UserName;
                    post.ProfilePicture = imagePath ?? post.ProfilePicture;
                    await _repository.Post.UpdatePostAsync(post);
                }
            }
        }

        private async Task UpdateFollowerOfUser(UserInfoDtoForUpdate userDtoForUpdate, string id, User userEntity)
        {
            if (string.IsNullOrWhiteSpace(userDtoForUpdate.Follower))
                return;

            if (userDtoForUpdate.Follower.Equals(id)) return;

            var userFollower = await _repository.User.GetUserAsync(userDtoForUpdate.Follower);

            if (userFollower == null)
                return;

            if (userEntity.Followers.Contains(userDtoForUpdate.Follower))
            {
                userFollower.Following.Remove(id);
                if (userFollower.Following.Count == 0) userFollower.Following = null;
                await _repository.User.UpdateUserAsync(userFollower);

                userEntity.Followers.Remove(userDtoForUpdate.Follower);
                var notification = (await _repository.Notification.GetNotificationsOfUser(new(), userEntity.UserId, userEntity.UserId, "FollowRequest", userFollower.UserId)).FirstOrDefault();
                if (notification != null) await _repository.Notification.DeleteNotification(notification.OwnerId, notification.CreatedAt);

            }
            else if (userDtoForUpdate.IsAcceptFollower != null && userEntity.FollowRequests.Any(followRequest => followRequest.FollowerId.Equals(userDtoForUpdate.Follower)))
            {
                if ((bool)userDtoForUpdate.IsAcceptFollower)
                {
                    var indexOfFollowRequest = userEntity.FollowRequests.FindIndex(0, userEntity.FollowRequests.Count, (followRequest => followRequest.FollowerId.Equals(userDtoForUpdate.Follower)));
                    if (indexOfFollowRequest >= 0)
                    {

                        userEntity.FollowRequests.RemoveAt(indexOfFollowRequest);
                        userFollower.Following.Add(id);
                        userEntity.Followers.Add(userFollower.UserId);
                    }
                }
                else
                {
                    var indexOfFollowRequest = userEntity.FollowRequests.FindIndex(0, userEntity.FollowRequests.Count, (followRequest => followRequest.FollowerId.Equals(id)));
                    if (indexOfFollowRequest >= 0)
                    {
                        userEntity.FollowRequests.RemoveAt(indexOfFollowRequest);
                        var notification = (await _repository.Notification.GetNotificationsOfUser(new(), userEntity.UserId, userEntity.UserId, "FollowRequest", userFollower.UserId)).FirstOrDefault();
                        if (notification != null) await _repository.Notification.DeleteNotification(notification.OwnerId, notification.CreatedAt);
                    }
                }
            }
            await _repository.User.UpdateUserAsync(userFollower);
        }

        private async Task UpdateFollowingOfUser(UserInfoDtoForUpdate userDtoForUpdate, string id, User userEntity)
        {
            if (string.IsNullOrWhiteSpace(userDtoForUpdate.Follow))
                return;

            if (userDtoForUpdate.Follow.Equals(id))
                return;

            var userFollowing = await _repository.User.GetUserAsync(userDtoForUpdate.Follow);
            if (userFollowing == null) return;

            if (userEntity.Following.Contains(userDtoForUpdate.Follow))
            {
                userFollowing.Followers.Remove(id);
                userEntity.Following.Remove(userDtoForUpdate.Follow);
                var notification = (await _repository.Notification.GetNotificationsOfUser(new(), userFollowing.UserId, userFollowing.UserId, "FollowRequest", userEntity.UserId)).FirstOrDefault();
                if (notification != null) await _repository.Notification.DeleteNotification(notification.OwnerId, notification.CreatedAt);
            }
            else
            {
                if (!userFollowing.Private)
                {
                    userFollowing.Followers.Add(id);
                    userEntity.Following.Add(userDtoForUpdate.Follow);
                    await _repository.Notification.CreateNotification(new Notification()
                    {
                        ActionType = "FollowRequest",
                        CreatedAt = DateTime.UtcNow.ToString(),
                        AffectedObjectId = userFollowing.UserId,
                        IsSeen = false,
                        OwnerId = userFollowing.UserId,
                        Metadata = new NotificationMetadata()
                        {
                            LastestActionUserId = userEntity.UserId,
                            ObjectId = "none",
                            ActionCount = 0,
                            LinkToAffectedObject = "none",
                        }
                    });
                }
                else
                {
                    var isSendFollowRequest = userFollowing.FollowRequests.Any(followRequest => followRequest.FollowerId.Equals(id));
                    if (isSendFollowRequest)
                    {
                        var indexOfFollowRequest = userFollowing.FollowRequests.FindIndex(0, userFollowing.FollowRequests.Count, (followRequest => followRequest.FollowerId.Equals(id)));
                        if (indexOfFollowRequest >= 0)
                        {

                            userFollowing.FollowRequests.RemoveAt(indexOfFollowRequest);
                            var notification = (await _repository.Notification.GetNotificationsOfUser(new(), userFollowing.UserId, userFollowing.UserId, "FollowRequest", userEntity.UserId)).FirstOrDefault();
                            if (notification != null) await _repository.Notification.DeleteNotification(notification.OwnerId, notification.CreatedAt);
                        }
                    }
                    else
                    {
                        userFollowing.FollowRequests.Add(new() { FollowerId = id, CreatedAt = DateTime.UtcNow });
                        await _repository.Notification.CreateNotification(new Notification()
                        {
                            ActionType = "FollowRequest",
                            CreatedAt = DateTime.UtcNow.ToString(),
                            AffectedObjectId = userFollowing.UserId,
                            IsSeen = false,
                            OwnerId = userFollowing.UserId,
                            Metadata = new NotificationMetadata()
                            {
                                LastestActionUserId = userEntity.UserId,
                                ObjectId = "none",
                                ActionCount = 0,
                                LinkToAffectedObject ="api/user",
                            }
                        });
                    }
                }
            }
            if (userFollowing.Followers.Count == 0) userFollowing.Followers = null;
            await _repository.User.UpdateUserAsync(userFollowing);
        }

        private async Task<(IEnumerable<ExpandoObject> users, MetaData metaData)> MappingUserEntityToDto(
            PagedList<User> users,
            string userId,
            string? fields)
        {
            var usersDto = _mapper.Map<List<UserDto>>(users).Select(u =>
            {
                u.NumberOfFollowing = u.Following.Count;
                u.NumberOfFollwers = u.Followers.Count;
                ConfigUserDto(ref u, userId);
                return u;
            }).ToList();
            var shapedData = _userShaper.ShapeData(usersDto, fields);

            return (users: shapedData, metaData: users.MetaData);
        }

        private void ConfigUserDto(ref UserDto userDto,
            string userId)
        {
            if (userDto.Private == true && !userDto.Followers.Contains(userId))
            {
                userDto.Followers = null;
                userDto.Following = null;
            }
            if (userDto.Followers != null && userDto.Followers.Contains(userId)) userDto.isFollowed = true;
            if (userDto.FollowRequests.Any(followRequest => followRequest.FollowerId.Equals(userId)))
            {
                userDto.FollowRequests = userDto.FollowRequests.Where(followRequest => followRequest.FollowerId.Equals(userId)).ToList();
            }
            else userDto.FollowRequests = null;
            userDto.PhoneNumber = null;
            userDto.BlockedUsers = null;
            userDto.DateOfBirth = null;
            userDto.LastModified = null;
            userDto.ConversationId = null;
        }

        public async Task<string> UploadUserImageAsync(
            string userId,
            FileInfo imageFile)
        {
            string fileName = $"users/{userId}/profile-picture.jpg";

            using (var fileStream = new FileStream(imageFile.FullName, FileMode.Open, FileAccess.Read))
            {
                var uploadRequest = new PutObjectRequest
                {
                    InputStream = fileStream,
                    BucketName = _s3Config.BucketName,
                    Key = fileName,
                    ContentType = "image/png"
                };

                uploadRequest.Headers.ContentDisposition = "inline";
                await _s3Client.PutObjectAsync(uploadRequest);
            }

            return _s3Config.S3BucketPath + fileName;
        }
    }
}