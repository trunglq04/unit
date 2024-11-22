using Amazon.DynamoDBv2.DataModel;

namespace Unit.Entities.Models
{
    [DynamoDBTable("Notifications")]
    public class Notification
    {
        [DynamoDBHashKey("owner_id")]
        // Id của chủ sở hữu phần bài viêt/ bình luận bị tác động
        public string OwnerId { get; set; }

        [DynamoDBProperty("action_type")]
        // CommentPost, LikePost, ReplyComment, LikeComment, FollowRequest
        public string ActionType { get; set; }

        [DynamoDBProperty("affected_object_id")]
        // Id của phần bị tác động: id của bài viết hoặc id của bình luận
        public string? AffectedObjectId { get; set; }  // For getting Post Content
        
        [DynamoDBProperty("created_at")]        
        public DateTime CreatedAt { get; set; }

        [DynamoDBProperty("metadata")]
        public NotificationMetadata Metadata { get; set; } = new();

        [DynamoDBProperty("is_seen")]
        public bool IsSeen { get; set; } = false;

    }
}
