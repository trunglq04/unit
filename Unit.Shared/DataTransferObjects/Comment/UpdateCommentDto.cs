using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unit.Shared.DataTransferObjects.Comment
{
    public class UpdateCommentDto
    {
        public string? PostId { get; set; }

        public string? CommentId { get; set; }

        public required string Content { get; set; }

        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        public string? ParentCommentId { get; set; }

        public List<AttachmentDto>? Attachments { get; set; } = new();

        public List<string>? Mentions { get; set; } = new();

        public MetadataDto? Metadata { get; set; } = new()
        {
            IsEdited = true,
        };

        public List<string>? Tags { get; set; } = new();
    }
}
