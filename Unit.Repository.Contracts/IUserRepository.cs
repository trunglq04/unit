using Unit.Entities.Models;

namespace Unit.Repository.Contracts
{
    public interface IUserRepository
    {
        Task CreateUserAsync(User user);
    }
}
