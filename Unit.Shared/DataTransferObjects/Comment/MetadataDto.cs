namespace Unit.Shared.DataTransferObjects.Comment
{
    public class MetadataDto
    {
        public bool IsEdited { get; set; }
        public List<string> Likes { get; set; } = new(); // store like user ids
        public int Replies { get; set; }
    }
}
    