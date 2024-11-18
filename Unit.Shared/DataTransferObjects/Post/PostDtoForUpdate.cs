using System.ComponentModel.DataAnnotations;

namespace Unit.Shared.DataTransferObjects.Post
{
    public record PostDtoForUpdate
    {
        [Required]
        public required string UserId { get; init; }

        public string? Content { get; init; }

        public bool? Hidden { get; init; }

        public bool? Like { get; init; }
    }
}
