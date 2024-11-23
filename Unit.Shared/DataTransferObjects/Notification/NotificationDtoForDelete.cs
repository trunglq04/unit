using System.ComponentModel.DataAnnotations;

namespace Unit.Shared.DataTransferObjects.Notification
{
    public record NotificationDtoForDelete
    {
        [Required]
        public required string CreatedAt { get; init; }
    }
}
