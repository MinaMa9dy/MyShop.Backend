namespace MyShop.Application.Common.ResultPattern
{
    public class Meta
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public int PerPage { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)Total / PerPage);
        public bool HasNext => Page < TotalPages;
        public bool HasPrev => Page > 1;
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }  // "asc" | "desc"
    }
}
