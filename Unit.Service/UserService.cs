using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Util;
using AutoMapper;
using Microsoft.Extensions.Options;
using System.Dynamic;
using Unit.Entities.ConfigurationModels;
using Unit.Entities.Models;
using Unit.Repository.Contracts;
using Unit.Service.Contracts;
using Unit.Service.Helper;
using Unit.Shared.DataTransferObjects;
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

        public UserService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, IDataShaper<UserDto> userShaper, IAmazonS3 s3Client,
            IOptions<AWSConfiguration> configuration)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _userShaper = userShaper;
            _s3Client = s3Client;
            _s3Config = configuration.Value.S3Bucket!;
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

            var shapedData = _userShaper.ShapeData(userDto, parameters.Fields);

            return shapedData;
        }


        public async Task<(IEnumerable<ExpandoObject> users, MetaData metaData)> GetUsersAsync(UserParameters parameters, string token)
        {
            var userId = JwtHelper.GetPayloadData(token, "username");

            var usersWithMetaData = await _repository.User.GetUsersExceptAsync(parameters, userId!);

            return await MappingUserEntityToDto(usersWithMetaData, userId!, parameters.Fields);
        }

        public async Task<(IEnumerable<ExpandoObject> users, MetaData metaData)> GetUsersByIdsAsync(UserParameters parameters, string token, List<string> ids)
        {
            var userId = JwtHelper.GetPayloadData(token, "username");

            var usersWithMetaData = await _repository.User.GetUsersByIdsAsync(parameters, ids);

            return await MappingUserEntityToDto(usersWithMetaData, userId!, parameters.Fields);
        }

        public async Task UpdateUser(UserInfoDtoForUpdate userDtoForUpdate, string id, string? imagePath = null)
        {
            var userEntity = await _repository.User.GetUserAsync(id);

            _mapper.Map(userDtoForUpdate, userEntity);
            if (!string.IsNullOrWhiteSpace(imagePath)) userEntity.ProfilePicture = imagePath;
            userEntity.LastModified = DateTime.UtcNow;
            await _repository.User.UpdateUserAsync(userEntity);

        }

        private async Task<(IEnumerable<ExpandoObject> users, MetaData metaData)> MappingUserEntityToDto(PagedList<User> users, string userId, string? fields)
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

        private void ConfigUserDto(ref UserDto userDto, string userId)
        {
            if (userDto.Private == true && !userDto.Followers.Contains(userId))
            {
                userDto.Followers = null;
                userDto.Following = null;
            }
            userDto.PhoneNumber = null;
            userDto.BlockedUsers = null;
            userDto.DateOfBirth = null;
            userDto.FollowRequests = null;
            userDto.LastModified = null;
            userDto.ConversationId = null;
        }

        public async Task<string> UploadUserImageAsync(string userId, FileInfo imageFile)
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
