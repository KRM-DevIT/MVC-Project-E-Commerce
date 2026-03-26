using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MiniECommerce.Areas.Admin.ViewModels.CategoryViewModels
{
    public class CategoryViewModel
    {
       
        [StringLength(100)]
        [Required]
        public string CategoryName { get; set; } = null!;
        public int? ParentCategoryId { get; set; }

        public List<SelectListItem>? Categories { get; set; }
    }
}
