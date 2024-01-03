namespace Hw10.ViewModels
{
    public class IndexViewModel<T> where T : class
    {
        public IEnumerable<T> Items { get; }
        public PaginationViewModel Pagination { get; set; }

        public IndexViewModel(IEnumerable<T> items, PaginationViewModel pagination)
        {
            Items = items;
            Pagination = pagination;
        }
    }
}
