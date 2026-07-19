using System.ComponentModel.DataAnnotations;

namespace MiniECommerce.Areas.Customer.ViewModels
{
    public class addressVM
    {

        public int AddressId { get; set; }


        [Required]
        [StringLength(100)]
        public string Street { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string City { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Country { get; set; } = null!;

        public string? Zip { get; set; }

        public bool IsDefault { get; set; }

    }
}
