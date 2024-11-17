namespace Unit.Shared.DataTransferObjects.Reply
{
    public class DeleteReplyDto
    {
        public required string ReplyId { get; set; }
        public required string AuthorId { get; set; }
    }
}
