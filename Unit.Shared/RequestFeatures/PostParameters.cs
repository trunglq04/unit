namespace Unit.Shared.RequestFeatures
{
    public class PostParameters : RequestParameters
    {
        public string? UserId { get; set; }

        public bool? IsHidden { get; set; }

        public string? PostId { get; set; }

        public bool? MyPost { get; set; }

    }
}
