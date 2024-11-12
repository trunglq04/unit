namespace Unit.Shared.RequestFeatures
{
    public class MetaData
    {
        public string? NexPageKey { get; set; }
        public int PageSize { get; set; }
        public bool HasNext => NexPageKey != null;
    }
}
