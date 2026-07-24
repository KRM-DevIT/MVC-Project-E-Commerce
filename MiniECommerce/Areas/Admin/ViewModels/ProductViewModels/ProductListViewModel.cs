using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MiniECommerce.Areas.Admin.ViewModels.ProductViewModels
{
    public class ProductListViewModel
    {
       
            public List<Product> Products { get; set; } = new();
            public int CurrentPage { get; set; }
            public int PageSize { get; set; }
            public int TotalCount { get; set; }
            public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
            
            public string categoryName { get; set; } = string.Empty;
        public bool HasPrevious => CurrentPage > 1;
            public bool HasNext => CurrentPage < TotalPages;
        
    }
}
