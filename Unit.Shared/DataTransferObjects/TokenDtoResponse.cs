using System.ComponentModel.DataAnnotations;

namespace Unit.Shared.DataTransferObjects
{
    public record TokenDtoResponse
    {
        [Required]
        public required string IdToken { get; init; }

        [Required]
        public required string AccessToken { get; init; }

        [Required]
        public required string RefreshToken { get; init; }

        [Required]
        public required int ExpiresIn { get; init; }
    }
}
