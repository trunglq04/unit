using Microsoft.VisualBasic;

namespace Unit.Shared.DataTransferObjects
{
    public class PostDto
    {
        public required string UserId { get; set; }

        public required string PostId { get; set; }

        public string Content { get; set; }

        public List<string> Media { get; set; } = new();

        public DateTime CreatedAt { get; set; }

        public DateTime LastModified { get; set; }

        public bool IsHidden { get; set; } = false;

        public int LikeCount { get; set; }

        public int CommentCount { get; set; }

        public List<Interaction> Reactions { get; set; } = new();
    }
}
