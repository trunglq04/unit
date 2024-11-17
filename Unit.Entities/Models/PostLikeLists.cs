using Amazon.DynamoDBv2.DataModel;

namespace Unit.Entities.Models
{
    [DynamoDBTable("PostLikeLists")]
    public class PostLikeList
    {
        [DynamoDBHashKey("post_id")] // Partition Key
        public required string PostId { get; set; }

        [DynamoDBRangeKey("user_id")]
        public required string UserId { get; set; }
    }
}
