using Amazon.DynamoDBv2.DataModel;

namespace Unit.Entities.Models
{
    public class Metadata
    {
        [DynamoDBProperty("is_edited")]
        public bool IsEdited { get; set; } = false;

        [DynamoDBProperty("likes")]
        public int Likes { get; set; }

        [DynamoDBProperty("replies")]
        public int Replies { get; set; }
    }
}
