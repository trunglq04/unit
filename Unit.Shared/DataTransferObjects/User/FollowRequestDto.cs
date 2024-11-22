namespace Unit.Shared.DataTransferObjects.User
{
    public class FollowRequestDto
    {
        public required string FollowerId { get; init; }

        public string? UserName { get; set; }

        public string? PictureProfile { get; set; }

        public required DateTime CreatedAt { get; set; }
    }
}
