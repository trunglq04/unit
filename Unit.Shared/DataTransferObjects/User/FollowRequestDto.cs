namespace Unit.Shared.DataTransferObjects.User
{
    public record FollowRequestDto
    {
        public required string FollowerId { get; init; }

        public required DateTime CreatedAt { get; set; }

        public string Status { get; init; } = "pending"; // "pending", "accepted", "rejected", "canceled"
    }
}
