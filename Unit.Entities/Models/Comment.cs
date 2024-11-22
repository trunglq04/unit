using Amazon.DynamoDBv2.DataModel;

namespace Unit.Entities.Models
{
    [DynamoDBTable("Comments")]
    public class Comment
    {
        [DynamoDBHashKey("post_id")] // Partition Key
        public string PostId { get; set; }

        [DynamoDBRangeKey("comment_id")]
        public required string CommentId { get; set; } = Guid.NewGuid().ToString();

        [DynamoDBProperty("user_id")]
        public required string UserId { get; set; }

        [DynamoDBProperty("content")]
        public required string Content { get; set; }

        [DynamoDBProperty("create_at")]
        public DateTime CreatedAt { get; set; }

        [DynamoDBProperty("last_modified")]
        public DateTime LastModified { get; set; }

        [DynamoDBProperty("parent_comment_id")]
        public string? ParentCommentId { get; set; } // null if this is a top-level comment

        [DynamoDBProperty("reactions")]
        public List<Interaction> Reactions { get; set; } = new();
    }
}
