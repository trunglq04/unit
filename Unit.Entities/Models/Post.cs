using Amazon.DynamoDBv2.DataModel;
using System.ComponentModel.DataAnnotations;

namespace Unit.Entities.Models
{
    [DynamoDBTable("Posts")]
    public class Post
    {
        [DynamoDBHashKey("user_id")] // Partition Key
        public required string UserId { get; set; }

        [DynamoDBRangeKey("post_id")]
        public required string PostId { get; set; } = Guid.NewGuid().ToString();

        [DynamoDBProperty("content")]
        [MaxLength(500)]
        public string Content { get; set; }

        [DynamoDBProperty("media")]
        public List<string> Media { get; set; } = new(); //link to video or image

        [DynamoDBProperty("create_at")]
        public DateTime CreatedAt { get; set; }

        [DynamoDBProperty("last_modified")]
        public DateTime LastModified { get; set; }

        [DynamoDBProperty("is_hidden")]
        public bool IsHidden { get; set; } = false;

        [DynamoDBProperty("is_private")]
        public bool IsPrivate { get; set; } = false;

        [DynamoDBProperty("like_count")]
        public int LikeCount { get; set; }

        [DynamoDBProperty("comment_count")]
        public int CommentCount { get; set; }

        [DynamoDBProperty("reactions")]
        public List<Interaction> Reactions { get; set; } = new();
    }
}
