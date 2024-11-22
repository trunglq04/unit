using Amazon.DynamoDBv2.DataModel;

namespace Unit.Entities.Models
{
    public class NotificationMetadata
    {
        // Tác nhân tạo ra hành động
        [DynamoDBProperty("latest_action_user_id")]
        public string? LastestActionUserId { get; set; }

        // Tổng số lần hành động này được thực hiện: số like.
        [DynamoDBProperty("action_count")]
        public int? ActionCount { get; set; }   // comment count or like count

        // Đường dẫn đến đối tượng bị tác động: bài viết hoặc bình luận
        [DynamoDBProperty("link_to_affected_object")]
        public string? LinkToAffectedObject { get; set; } 
    }
}
