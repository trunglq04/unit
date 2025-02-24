using System.ComponentModel.DataAnnotations;
using Unit.Shared.CustomValidationAttribute;

namespace Unit.Shared.DataTransferObjects.Authentication
{
    public record ResetPasswordDtoRequest : IPasswordConfirmation
    {
        [Required]
        public required string Code { get; init; }

        [Required]
        public required string Email { get; init; }

        [Required]
        [PasswordValidation]
        public required string Password { get; init; }

        [Required]
        public required string ConfirmPassword { get; init; }
    }
}
