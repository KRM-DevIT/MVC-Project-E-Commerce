using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniECommerce.Models
{
    public class Category
    {

        // Main-properties 

        [Key]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = null!;

        // nav properties

        [ForeignKey(nameof(ParentCategory))]
        public int? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }
        
        public ICollection<Category> ChildrenCategories { get; set; } = new List<Category>();

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
