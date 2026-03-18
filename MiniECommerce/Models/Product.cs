using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace MiniECommerce.Models
{
    public class Product
    {
        //Main properties of the product
        [Key]
        public int ProductId { get; set; }

        [Required(ErrorMessage="Product name is required.")]
        [StringLength(100, ErrorMessage="Product name cannot exceed 100 characters.")]
        public string ProductName { get; set; } = null!;

        [Required(ErrorMessage="Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        [DataType(DataType.Currency)]
        [Precision(10, 2)]
        public decimal CurrentPrice { get; set; }

        [StringLength(500, ErrorMessage="Description cannot exceed 500 characters." )]
        public string? Description { get; set; }

        [StringLength(50,ErrorMessage ="SKU cannot exceed 50 characters.")]
        // to be flagged unique in the on model creating method of the DbContext don't forget the filtered index to allow multiple nulls
        public string? SKU { get; set; } // Stock Keeping Unit, unique identifier for the product

        [DataType(DataType.ImageUrl)]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage="Stock quantity is required.")]
        [Range(0, 1000, ErrorMessage = "Stock quantity Must be greater than 0 and less than 1000")]
        public int StockQuantity { get; set; } // Must identify the current available quantity of product in the stock

        public bool IsActive { get; set; } = true; // Allowed to be shown on the system/Site for sale or not (it must be false if stock = 0) 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        // Navigation property for the category

        [Required]
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
