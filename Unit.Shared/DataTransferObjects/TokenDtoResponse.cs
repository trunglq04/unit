
namespace Unit.Shared.DataTransferObjects
{
    public record TokenDtoResponse(string IdToken, string AccessToken, string RefreshToken, int ExpiresIn);
}
