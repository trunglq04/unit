using Amazon.DynamoDBv2.DataModel;

namespace Unit.Entities.Models
{
    public class Reply
    {
        [DynamoDBProperty("author_id")]
        public required string AuthorId { get; set; }

        [DynamoDBProperty("reply_id")]
        public required string ReplyId { get; set; }

        [DynamoDBProperty("content")]
        public required string Content { get; set; }
    }
}
