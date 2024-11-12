using System.Dynamic;
using Unit.Shared.DataTransferObjects;
using Unit.Shared.RequestFeatures;

namespace Unit.Service.Contracts
{
    public interface IUserService
    {
        Task<(IEnumerable<ExpandoObject> users, MetaData metaData)> GetUsersAsync(UserParameters parameters, string token);

        Task<ExpandoObject> GetUserByIdAsync(UserParameters parameters, string id);

        Task<(IEnumerable<ExpandoObject> users, MetaData metaData)> GetUsersByIdsAsync(UserParameters parameters, string[] ids);

        Task UpdateUser(UserInfoDtoForUpdate userDtoForUpdate, string id);


    }
}
