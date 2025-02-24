using Amazon.DynamoDBv2.DataModel;

namespace Unit.Entities.Models
{
    public class Metadata
    {
        [DynamoDBProperty("is_edited")]
        public bool? IsEdited { get; set; } = false;

        [DynamoDBProperty("likes")]
        public List<string>? Likes { get; set; } = new();  // store like user ids
    }
}
