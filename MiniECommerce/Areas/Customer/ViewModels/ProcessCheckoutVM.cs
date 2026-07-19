using System.ComponentModel.DataAnnotations;

namespace MiniECommerce.Areas.Customer.ViewModels
{
    public class ProcessCheckoutVM
    {
        [Required]
        public int ShippingAddressId { get; set; }

    }
}
