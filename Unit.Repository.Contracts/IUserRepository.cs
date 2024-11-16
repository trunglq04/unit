using Unit.Entities.Models;
using Unit.Shared.RequestFeatures;

namespace Unit.Repository.Contracts
{
    public interface IUserRepository
    {
        Task<PagedList<User>> GetUsersExceptAsync(UserParameters userParameters, string userId);

        Task<User> GetUserAsync(string userId);

        Task<PagedList<User>> GetUsersByIdsAsync(UserParameters userParameters, List<string> ids);

        Task CreateUserAsync(User user);

        Task UpdateUserAsync(User user);
    }
}