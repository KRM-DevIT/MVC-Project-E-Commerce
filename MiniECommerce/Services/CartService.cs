using Microsoft.EntityFrameworkCore;
using MiniECommerce.Extensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniECommerce.Services
{
    public class CartItem
    {
        public int ProductId { get; set; }

        public decimal UnitPriceAtPurchase { get; set; } 

        public int Quantity { get; set; } 

        public decimal LineTotal { get; set; } 
        public int OrderId { get; set; }

        public CartItem(int ProductId, int OrderId , decimal UnitPrice , int Quantity)
        {
            this.ProductId = ProductId;
            this.OrderId = OrderId;
            this.UnitPriceAtPurchase = UnitPrice;
            this.Quantity = Quantity;
            this.LineTotal = UnitPrice*Quantity;    
        }


    }
    public class Cart
    {
       public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        
    }
    public class CartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private Cart GetCart()
        {
            var session = _httpContextAccessor.HttpContext!.Session;

            var cart = session.("Cart");

            if (cart == null)
                cart = new Cart();

            return cart;
        }

        private void SaveCart(Cart cart)
        {
            var session = _httpContextAccessor.HttpContext!.Session;

            session.SetCartCookie("Cart", cart);
        }

        public void AddToCart(OrderItem item)
        {
            var cart = GetCart();

            var existing = cart.CartItems
                .FirstOrDefault(x => x.ProductId == item.ProductId);

            if (existing != null)
                existing.Quantity += item.Quantity;
            else
                cart.CartItems.Add(item);

            SaveCart(cart);
        }
    }
}
