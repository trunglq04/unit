using Unit.Entities.Models;

namespace Unit.Repository.Contracts
{
    public interface IPostRepository
    {
        Task CreatePostAsync(Post post);

        Task UpdateUserAsync(Post post);
    }
}
