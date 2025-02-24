namespace Unit.Shared.DataTransferObjects.Reply
{
    public class ReplyDto
    {
        public required string AuthorId { get; set; }
        public required string ReplyId { get; set; }
        public required string Content { get; set; }
    }
}
