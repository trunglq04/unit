using Amazon.DynamoDBv2.DataModel;

namespace Unit.Entities.Models
{
    [DynamoDBTable("Posts")]
    public class Post
    {
        [DynamoDBHashKey("user_id")] // Partition Key
        public string UserId { get; set; }

        [DynamoDBRangeKey("post_id")]
        public string PostId { get; set; } = Guid.NewGuid().ToString();

        [DynamoDBProperty("content")]
        public string Content { get; set; }

        [DynamoDBProperty("media")]
        public List<string> Media { get; set; } = new List<string>(); //link to video or image

        [DynamoDBProperty("create_at")]
        public DateTime CreatedAt { get; set; }

        [DynamoDBProperty("last_modified")]
        public DateTime LastModified { get; set; }

        [DynamoDBProperty("private")]
        public bool Private { get; set; }

        [DynamoDBProperty("like_count")]
        public int LikeCount { get; set; }

        [DynamoDBProperty("comment_count")]
        public int CommentCount { get; set; }

        [DynamoDBProperty("reactions")]
        public List<Interaction> Reactions { get; set; } = new List<Interaction>();
    }
}
