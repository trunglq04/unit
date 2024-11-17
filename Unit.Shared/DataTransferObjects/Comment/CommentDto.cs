namespace Unit.Shared.DataTransferObjects.Comment
{
    public class CommentDto
    {
        public string? AuthorId { get; set; }
        public string? PostId { get; set; }

        public string? CommentId { get; set; }        // auto-generate

        public required string Content { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        public List<AttachmentDto>? Attachments { get; set; } = new();

        public List<string>? Mentions { get; set; } = new();

        public MetadataDto? Metadata { get; set; } = new() {
            IsEdited = false,
            Likes = new(),
            Replies = 0
        };

        public List<string>? Tags { get; set; } = new();
    }
}
