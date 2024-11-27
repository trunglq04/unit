namespace Unit.Shared.DataTransferObjects.Notification
{
    public class NotificationDto
    {
        public required string OwnerId { get; set; }

        public string? AffectedObjectId { get; set; }

        public required string ActionType { get; set; }

        public required string ActionContent { get; set; }

        public NotificationMetadataDto? Metadata { get; set; }

        public bool IsSeen { get; set; }

        public string CreatedAt { get; set; }
    }
}
