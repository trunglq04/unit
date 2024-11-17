namespace Unit.Shared.DataTransferObjects.Reply
{
    public class CreateReplyDto
    {
        public required string ReplyId { get; set; } = new Guid().ToString();
        public required string Content { get; set; }
    }
}
