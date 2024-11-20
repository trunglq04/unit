namespace Unit.Shared.RequestFeatures
{
    public class PagedList<T> : List<T>
    {
        public MetaData MetaData { get; set; }

        public PagedList(List<T> items, string? pageKey, int size, int pageNumber)
        {
            MetaData = new MetaData
            {
                NextPageKey = pageKey,
                PageSize = size,
                CurrentPage = pageNumber,
            };

            AddRange(items);
        }

    }
}
