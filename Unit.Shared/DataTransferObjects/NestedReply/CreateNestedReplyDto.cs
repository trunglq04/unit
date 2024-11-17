using Unit.Shared.DataTransferObjects.Reply;

namespace Unit.Shared.DataTransferObjects.NestedReply
{
    public class CreateNestedReplyDto
    {
        public required string AuthorId { get; set; }
        public required string ParentCommentId { get; set; }
        public List<ReplyDto> Replies { get; set; } = new();
    }
}
