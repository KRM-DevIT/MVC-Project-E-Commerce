using MiniECommerce.Models;
using MiniECommerce.Services;

namespace MiniECommerce.Areas.Customer.ViewModels
{
    public class CheckoutVM
    {
        public string UserId { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
        public int ShippingAddressId { get; set; }
        public List<Address> Addresses { get; set; } = new();
        public List<CartItem> CartItems { get; set; } = new();
        public decimal CartTotal { get; set; }
        public decimal Shipping { get; set; } = 5.99m;
        public decimal GrandTotal => CartTotal + Shipping;
    }
}
