namespace Unit.Shared.DataTransferObjects.Comment
{
    public class UpdateCommentDto
    {
        //public string AuthorId { get; set; }

        public string? PostId { get; set; }

        public string? CommentId { get; set; }

        public List<AttachmentDto>? Attachments { get; set; } = new();

        public required string Content { get; set; }

        //public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        public List<string>? Mentions { get; set; } = new();

        //public MetadataDto? Metadata { get; set; } = new()
        //{
        //    IsEdited = true,
        //};
        public List<string>? Tags { get; set; } = new();
    }
}
