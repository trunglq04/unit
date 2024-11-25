using System.ComponentModel.DataAnnotations;

namespace Unit.Shared.DataTransferObjects.Notification
{
    public record NotificationDtoForUpdate
    {
        [Required]
        public required string CreatedAt { get; init; }
    }
}
