namespace Unit.Shared.DataTransferObjects.Notification
{
    public class NotificationMetadataDto
    {
        public string? LastestActionUserId { get; set; }

        public string? ObjectId { get; set; }

        public string? UserName { get; set; }

        public string? ProfilePicture { get; set; }

        public string? LinkToAffectedObject { get; set; }

        public int ActionCount { get; set; }
    }
}
