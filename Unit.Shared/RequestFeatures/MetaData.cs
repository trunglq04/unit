namespace Unit.Shared.RequestFeatures
{
    public class MetaData
    {
        public string? NextPageKey { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public bool HasNext => NextPageKey != null;
    }
}
