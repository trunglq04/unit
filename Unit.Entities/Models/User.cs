using Amazon.DynamoDBv2.DataModel;

namespace Unit.Entities.Models
{
    [DynamoDBTable("Users")]
    public class User
    {
        [DynamoDBHashKey("user_name")] // Partition Key
        public string UserName { get; set; }

        [DynamoDBRangeKey("user_id")]
        public string UserId { get; set; }

        [DynamoDBProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [DynamoDBProperty("date_of_birth")]
        public DateTime DateOfBirth { get; set; }

        [DynamoDBProperty("profile_picture")]
        public string? ProfilePicture { get; set; }

        [DynamoDBProperty("bio")]
        public string? Bio { get; set; }

        [DynamoDBProperty("followers")]
        public List<string> Followers { get; set; } = new List<string>();

        [DynamoDBProperty("following")]
        public List<string> Following { get; set; } = new List<string>();

        [DynamoDBProperty("blocked_users")]
        public List<string> BlockedUsers { get; set; } = new List<string>();

        [DynamoDBProperty("status")]
        public bool Status { get; set; } // e.g., "active", "inactive"

        [DynamoDBProperty("private")]
        public bool Private { get; set; } // "public" or "private"

        [DynamoDBProperty("create_at")]
        public DateTime CreatedAt { get; set; }

        [DynamoDBProperty("last_modified")]
        public DateTime LastModified { get; set; }

        [DynamoDBProperty("follow_requests")]
        public List<FollowRequest> FollowRequests { get; set; } = new List<FollowRequest>();
    }
}
