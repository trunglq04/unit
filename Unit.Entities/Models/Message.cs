using Amazon.DynamoDBv2.DataModel;

namespace Unit.Entities.Models
{
    [DynamoDBTable("Messages")]
    public class Message
    {
        [DynamoDBHashKey("conversation_id")] // Partition Key
        public required string ConversationId { get; set; }//combine 2 user_id sender and reciver to have this id

        [DynamoDBRangeKey("message_id")]
        public string MessageId { get; set; } = Guid.NewGuid().ToString(); // Unique ID for each message

        [DynamoDBProperty("sender_id")]
        public required string SenderId { get; set; } // ID of the user sending the message

        [DynamoDBProperty("receiver_id")]
        public required string ReceiverId { get; set; } // ID of the user receiving the message

        [DynamoDBProperty("content")]
        public required string Content { get; set; } // Message content

        [DynamoDBProperty("status")]
        public bool Status { get; set; } // e.g., "unread", "read"

        [DynamoDBProperty("reply_to_message_id")]
        public string? ReplyToMessageId { get; set; } // ID of the message being replied to, if any

        [DynamoDBProperty("create_at")]
        public DateTime CreatedAt { get; set; }

        [DynamoDBProperty("last_modified")]
        public DateTime LastModified { get; set; }

        [DynamoDBProperty("reactions")]
        public List<string> Reactions { get; set; } = new(); // List of reactions on the message
    }
}
