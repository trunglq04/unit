﻿namespace Unit.Shared.DataTransferObjects.Comment
{
    public class CommentDto
    {
        public string? PostId { get; set; }

        public string? CommentId { get; set; } = Guid.NewGuid().ToString();         // auto-generate

        public required string Content { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        public string? ParentCommentId { get; set; } // null if this is a top-level comment

        public List<AttachmentDto>? Attachments { get; set; } = new();

        public List<string>? Mentions { get; set; } = new();

        public MetadataDto? Metadata { get; set; } = new() {
            IsEdited = false,
            Likes = 0,
            Replies = 0
        };

        public List<string>? Tags { get; set; } = new();
    }
}
