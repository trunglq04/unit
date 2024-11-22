namespace Unit.Shared.RequestFeatures
{
    public abstract class RequestParameters
    {
        const int maxSize = 50;

        private int _size = 10;

        public int Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = (value > maxSize) ? maxSize : value;
            }
        }
        public string? Page { get; set; }

        public string? OrderBy { get; set; }

        public string? Fields { get; set; }
    }
}
