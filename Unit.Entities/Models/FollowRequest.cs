using Amazon.DynamoDBv2.DataModel;

namespace Unit.Entities.Models
{
    public class FollowRequest
    {
        [DynamoDBProperty("follower_id")]
        public required string FollowerId { get; set; }

        [DynamoDBProperty("create_at")]
        public DateTime CreatedAt { get; set; }

        [DynamoDBProperty("last_modified")]
        public DateTime LastModified { get; set; }

        [DynamoDBProperty("status")]
        public required string Status { get; set; } = "pending"; // "pending", "accepted", "rejected", "canceled"
    }
}
