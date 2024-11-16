using Amazon.DynamoDBv2.DataModel;

namespace Unit.Entities.Models
{
    [DynamoDBTable("Comments")]
    public class Comment
    {
        [DynamoDBHashKey("post_id")] // Partition Key
        public required string PostId { get; set; }

        [DynamoDBRangeKey("comment_id")] // Sort Key
        public string CommentId { get; set; }

        [DynamoDBProperty("attachments")]
        public List<Attachment> Attachments { get; set; } = new();

        [DynamoDBProperty("author_id")]
        public string AuthorId { get; set; }

        [DynamoDBProperty("content")]
        public string Content { get; set; }

        [DynamoDBProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [DynamoDBProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [DynamoDBProperty("mentions")]
        public List<string> Mentions { get; set; } = new();

        [DynamoDBProperty("metadata")]
        public Metadata Metadata { get; set; } = new();

        [DynamoDBProperty("tags")]
        public List<string> Tags { get; set; } = new();

        [DynamoDBProperty("parent_comment_id")]
        public string? ParentCommentId { get; set; }            // null if this is a top-level comment
    }
}
