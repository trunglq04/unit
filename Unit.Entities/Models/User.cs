using Amazon.DynamoDBv2.DataModel;

namespace Unit.Entities.Models
{
    [DynamoDBTable("Users")]
    public class User
    {
        [DynamoDBHashKey("user_id")] // Partition Key
        public required string UserId { get; set; }

        [DynamoDBProperty("user_name")]
        public required string UserName { get; set; }

        [DynamoDBProperty("phone_number")]
        public string? PhoneNumber { get; set; }

        [DynamoDBProperty("date_of_birth")]
        public DateTime DateOfBirth { get; set; }

        [DynamoDBProperty("profile_picture")]
        public string? ProfilePicture { get; set; }

        [DynamoDBProperty("bio")]
        public string? Bio { get; set; }

        [DynamoDBProperty("followers")]
        public List<string>? Followers { get; set; } = new();

        [DynamoDBProperty("following")]
        public List<string>? Following { get; set; } = new();

        [DynamoDBProperty("blocked_users")]
        public List<string>? BlockedUsers { get; set; } = new List<string>();

        [DynamoDBProperty("conversation_id")]
        public List<string>? ConversationId { get; set; } = new List<string>();

        [DynamoDBProperty("active")]
        public bool Active { get; set; } // e.g., "active", "inactive"

        [DynamoDBProperty("private")]
        public bool Private { get; set; } = false; // "public" or "private"

        [DynamoDBProperty("create_at")]
        public DateTime CreatedAt { get; set; }

        [DynamoDBProperty("last_modified")]
        public DateTime LastModified { get; set; }

        [DynamoDBProperty("follow_requests")]
        public List<FollowRequest> FollowRequests { get; set; } = new();
    }
}
