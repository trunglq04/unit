namespace Unit.Shared.DataTransferObjects.Reply
{
    public class CreateReplyDto
    {
        public string? ReplyId { get; set; } = Guid.NewGuid().ToString();
        public required string Content { get; set; }
    }
}
