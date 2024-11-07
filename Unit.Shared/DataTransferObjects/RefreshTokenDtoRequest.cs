using System.ComponentModel.DataAnnotations;


namespace Unit.Shared.DataTransferObjects
{
    public record RefreshTokenDtoRequest
    {
        [Required]
        public required string RefreshToken { get; init; }
    }
}
