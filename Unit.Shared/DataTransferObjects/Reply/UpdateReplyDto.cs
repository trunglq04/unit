namespace Unit.Shared.DataTransferObjects.Reply
{
    public class UpdateReplyDto
    {
        public string? AuthorId { get; set; }
        public required string ReplyId { get; set; }
        public required string Content { get; set; }
    }
}
