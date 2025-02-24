namespace Unit.Shared.DataTransferObjects.Reply
{
    public class ResponseReplyDto
    {
        public required string AuthorId { get; set; }
        public string AuthorUserName { get; set; }
        public string AuthorProfilePicture  { get; set; }
        public required string ReplyId { get; set; }
        public required string Content { get; set; }
    }
}
