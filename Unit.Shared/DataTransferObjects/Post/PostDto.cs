namespace Unit.Shared.DataTransferObjects.Post
{
    public class PostDto
    {
        public required string UserId { get; set; }

        public required string UserName { get; set; }

        public string? ProfilePicture { get; set; }

        public required string PostId { get; set; }

        public required string Content { get; set; }

        public bool IsLiked { get; set; } = false;

        public List<string> Media { get; set; } = new();

        public DateTime CreatedAt { get; set; }

        public DateTime LastModified { get; set; }

        public bool IsHidden { get; set; } = false;

        public int LikeCount { get; set; }

        public int CommentCount { get; set; }
    }
}
