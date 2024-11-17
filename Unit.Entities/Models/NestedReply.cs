

using Amazon.DynamoDBv2.DataModel;

namespace Unit.Entities.Models
{
    [DynamoDBTable("NestedReplies")]
    public class NestedReply
    {
        [DynamoDBHashKey("post_id")] // Partition Key
        public required string PostId { get; set; }

        [DynamoDBRangeKey("parent_comment_id")] // Sort Key
        public required string ParentCommentId { get; set; }

        [DynamoDBProperty("replies")]
        public List<Reply> Replies { get; set; } = new();
    }
}
