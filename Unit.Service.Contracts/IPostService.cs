using Unit.Entities.Models;
using Unit.Shared.DataTransferObjects.Post;
using Unit.Shared.RequestFeatures;

namespace Unit.Service.Contracts
{
    public interface IPostService
    {
        Task<string> UploadMediaPostAsync(string userId, Stream fileStream, string fileExtension);

        Task CreatePost(PostDtoForCreation post, string userId, List<string>? mediaPath = null);

        Task<(IEnumerable<PostDto> posts, MetaData metaData)> GetPosts(PostParameters request, string token);

        Task<(IEnumerable<PostLikeList> postLikeLists, MetaData metaData)> GetListLikedPost(PostParameters request);

        Task UpdatePost(string postId, PostDtoForUpdate post, string token, bool comment = false);

    }
}
