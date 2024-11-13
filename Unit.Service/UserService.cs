using AutoMapper;
using System.Dynamic;
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

        public UserService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, IDataShaper<UserDto> userShaper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _userShaper = userShaper;
        }

        public async Task<ExpandoObject> GetUserByIdAsync(UserParameters parameters, string id)
        {
            var users = await _repository.User.GetUserAsync(id!);

            var usersDto = _mapper.Map<UserDto>(users);

            var shapedData = _userShaper.ShapeData(usersDto, parameters.Fields);

            return shapedData;
        }


        public async Task<(IEnumerable<ExpandoObject> users, MetaData metaData)> GetUsersAsync(UserParameters parameters, string token)
        {

            var userId = JwtHelper.GetPayloadData(token, "username");

            var usersWithMetaData = await _repository.User.GetUsersExceptAsync(parameters, userId!);

            var usersDto = _mapper.Map<List<UserDto>>(usersWithMetaData).Where(u => u.Active == true).Select(u =>
            {
                u.NumberOfFollowing = u.Following.Count;
                u.NumberOfFollwers = u.Followers.Count;
                if (u.Private == true && !u.Followers.Contains(userId))
                {
                    u.Followers = [];
                    u.Following = [];
                    u.PhoneNumber = "";
                }
                u.BlockedUsers = [];
                return u;
            });

            var shapedData = _userShaper.ShapeData(usersDto, parameters.Fields);

            return (users: shapedData, metaData: usersWithMetaData.MetaData);
        }

        public async Task<(IEnumerable<ExpandoObject> users, MetaData metaData)> GetUsersByIdsAsync(UserParameters parameters, string[] ids)
        {
            var usersWithMetaData = await _repository.User.GetUsersByIdsAsync(parameters, ids);

            var usersDto = _mapper.Map<List<UserDto>>(usersWithMetaData);

            var shapedData = _userShaper.ShapeData(usersDto, parameters.Fields);

            return (users: shapedData, metaData: usersWithMetaData.MetaData);
        }

        public async Task UpdateUser(UserInfoDtoForUpdate userDtoForUpdate, string id)
        {
            var userEntity = await _repository.User.GetUserAsync(id);

            _mapper.Map(userDtoForUpdate, userEntity);

            userEntity.LastModified = DateTime.UtcNow;
            await _repository.User.UpdateUserAsync(userEntity);

        }
    }
}
