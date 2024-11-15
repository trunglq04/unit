using Amazon.DynamoDBv2.DataModel;

namespace Unit.Entities.Models
{
    public class Attachment
    {
        [DynamoDBProperty("type")]
        public string Type { get; set; }

        [DynamoDBProperty("url")]
        public string Url { get; set; }
    }
}
