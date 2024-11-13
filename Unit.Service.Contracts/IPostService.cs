using Unit.Shared.DataTransferObjects;

namespace Unit.Service.Contracts
{
    public interface IPostService
    {
        Task<string> UploadMediaPostAsync(string userId, Stream fileStream, string fileExtension);

        Task CreatePost(PostDtoForCreation post, string userId, List<string>? mediaPath = null);
    }
}
