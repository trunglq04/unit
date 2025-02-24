using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace Unit.Shared.DataTransferObjects.Authentication
{
    public record TokenDtoResponse
    {
        [Required]
        public required string IdToken { get; init; }

        [Required]
        public required string AccessToken { get; init; }

        [Required]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public required string RefreshToken { get; init; }

        [Required]
        public required int ExpiresIn { get; init; }
    }
}
