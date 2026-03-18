using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MiniECommerce.Models.IdentityModels;
namespace MiniECommerce.Models
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }

        [Required]
        [StringLength(100,ErrorMessage ="Street Can't Exceed 100 characters")]
        public string Street { get; set; } = null!;

        [Required]
        [StringLength(50,ErrorMessage ="City Cant' Exceed 50 chars")]
        public string City { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Country { get; set; } = null!;

        [Display(Name = "Zip Code")]
        public string? Zip { get; set; } = null!;

        public bool IsDefault { get; set; } // if the user have many addresses one of them is selectedDefault

        // Nav Properties

        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;
        public ApplicationUser? User { get; set; }
        public ICollection<Order> Orders { get; set; }  = new List<Order>();
    }
}
