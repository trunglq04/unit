using Unit.Entities.Models;
using Unit.Shared.DataTransferObjects;
using Unit.Shared.RequestFeatures;

namespace Unit.Repository.Contracts
{
    public interface IPostRepository
    {
        Task CreatePostAsync(Post post);

        Task UpdateUserAsync(Post post);

        Task<PagedList<Post>> GetPosts(PostParameters request, List<string>? userFollowing = null);

        Task<PagedList<Post>> GetPostsByUserId(PostParameters request);


    }
}
