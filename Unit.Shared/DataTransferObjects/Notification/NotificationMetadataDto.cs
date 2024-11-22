namespace Unit.Shared.DataTransferObjects.Notification
{
    public class NotificationMetadataDto
    {
        public required string LastedActionUserId { get; set; }

        public string? ObjectId { get; set; }

        public required string UserName { get; set; }

        public string? ProfilePicture { get; set; }

        public required string LinkToAffectedObject { get; set; }

        public int ActionCount { get; set; }
    }
}
