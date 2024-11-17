using Unit.Shared.DataTransferObjects.Reply;

namespace Unit.Shared.DataTransferObjects.NestedReply
{
    public class NestedReplyDto
    {
        public required string PostId { get; set; }
        public required string ParentCommentId { get; set; }
        public List<ReplyDto> Replies { get; set; } = new();
    }
}
