using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniECommerce.Areas.Admin.ViewModels.ProductViewModels
{
    public class ProductCreateViewModel
    {

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        public string ProductName { get; set; } = null!;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        [DataType(DataType.Currency)]
        [Precision(10, 2)]
        public decimal CurrentPrice { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [StringLength(50, ErrorMessage = "SKU cannot exceed 50 characters.")]
        public string? SKU { get; set; }

        [Display(Name = "Product Image")]
        public IFormFile? ImageFile { get; set; }

        [Required(ErrorMessage = "Stock quantity is required.")]
        [Range(0, 1000, ErrorMessage = "Stock quantity Must be greater than 0 and less than 1000")]
        public int StockQuantity { get; set; } 

        public bool IsActive { get; set; } = true; 

        [Required]
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        public List<SelectListItem>? categories { get; set; }
    }
}
