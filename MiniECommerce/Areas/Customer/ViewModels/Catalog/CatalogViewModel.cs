namespace MiniECommerce.Areas.Customer.ViewModels.Catalog
{
    public class CatalogViewModel
    {
        public List<Category> Categories { get; set; }
        public List<Product> Products { get; set; }
        public List<int> SelectedCategories { get; set; } = new(); 

        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
        public List<int> CartProductIds { get; set; } = new();

    }
}
