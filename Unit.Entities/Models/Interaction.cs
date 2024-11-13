using Amazon.DynamoDBv2.DataModel;

namespace Unit.Entities.Models
{
    public class Interaction
    {
        [DynamoDBProperty("user_id")]
        public required string UserId { get; set; } // ID of the user who made the interaction

        [DynamoDBProperty("type")]
        public required string Type { get; set; } // e.g., "like", "love", etc.

        [DynamoDBProperty("create_at")]
        public DateTime CreatedAt { get; set; }

    }
}
