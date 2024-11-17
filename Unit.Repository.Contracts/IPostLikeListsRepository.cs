using Unit.Entities.Models;
using Unit.Shared.RequestFeatures;

namespace Unit.Repository.Contracts
{
    public interface IPostLikeListsRepository
    {
        Task CreatePostLikeListAsync(PostLikeList postLikeList);

        Task RemovePostLikeListAsync(PostLikeList postLikeList);

        Task<bool> IsLikedPost(string postId, string userId);

        Task<PagedList<PostLikeList>> GetPostLikedListsAsync(PostParameters request);
    }
}
