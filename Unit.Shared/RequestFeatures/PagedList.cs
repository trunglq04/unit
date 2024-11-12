namespace Unit.Shared.RequestFeatures
{
    public class PagedList<T> : List<T>
    {
        public MetaData MetaData { get; set; }

        public PagedList(List<T> items, string? pageKey, int size)
        {
            MetaData = new MetaData
            {
                NexPageKey = pageKey,
                PageSize = size
            };

            AddRange(items);
        }

    }
}
