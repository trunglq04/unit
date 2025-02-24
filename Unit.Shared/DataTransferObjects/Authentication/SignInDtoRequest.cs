
using System.ComponentModel.DataAnnotations;

namespace Unit.Shared.DataTransferObjects.Authentication
{
    public record SignInDtoRequest
    {
        [Required]
        public required string Email { get; init; }

        [Required]
        public required string Password { get; init; }
    }
}
