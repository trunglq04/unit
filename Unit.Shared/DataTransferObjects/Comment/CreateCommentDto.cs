using System.ComponentModel.DataAnnotations;

namespace Unit.Shared.DataTransferObjects.Comment
{
    public class CreateCommentDto
    {
        public string? PostId { get; set; }

        [Required]
        public required string UserId { get; set; }

        public string? CommentId { get; set; } = Guid.NewGuid().ToString();         // auto-generate

        public required string Content { get; set; } = string.Empty;

        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public List<AttachmentDto>? Attachments { get; set; } = new();

        public List<string>? Mentions { get; set; } = new();

        public MetadataDto? Metadata { get; set; } = new()
        {
            IsEdited = false,
            Likes = new(),
            Replies = 0
        };

        public List<string>? Tags { get; set; } = new();
    }
}
