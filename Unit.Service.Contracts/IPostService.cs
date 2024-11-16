using Unit.Shared.DataTransferObjects.Post;
using Unit.Shared.RequestFeatures;

namespace Unit.Service.Contracts
{
    public interface IPostService
    {
        Task<string> UploadMediaPostAsync(string userId, Stream fileStream, string fileExtension);

        Task CreatePost(PostDtoForCreation post, string userId, List<string>? mediaPath = null);

        Task<(IEnumerable<PostDto> posts, MetaData metaData)> GetPosts(PostParameters request, string token);
    }
}
