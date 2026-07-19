using Microsoft.EntityFrameworkCore;
using MiniECommerce.Extensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniECommerce.Services
{
    public class CartItem
    {
        public int itemId { get; set; }
        public string ProductName {  get; set; }
        public decimal UnitPriceAtPurchase { get; set; } 
        public int Quantity { get; set; } 
        public decimal LineTotal { get; set; }  
        public StockStatus StockStatus { get; set; }
        public string? ImageUrl { get; set; }
        public string DeliveryDate { get; set; } = null!;

        public int AvailableStock { get; set; }
    }
    public class Cart
    {
        public Dictionary<int, CartItem> CartItems { get; set; } = new Dictionary<int, CartItem>();

    }


    public class CartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductService _productService;
        public CartService(IHttpContextAccessor httpContextAccessor , IProductService productService)
        {
            _httpContextAccessor = httpContextAccessor;
            _productService = productService;
        }
        private Cart GetCart()
        {
            var session = _httpContextAccessor.HttpContext!.Session;

            var cart = session.GetCartCookie("Cart");

            if (cart == null)
                cart = new Cart(); 

            return cart;
        }
        private void SaveCart(Cart cart)
        {
            var session = _httpContextAccessor.HttpContext!.Session;

            session.SetCartCookie("Cart", cart);
        }
        public List<CartItem> GetAllCartItems()
        {
            var cart = GetCart(); // if not found won't make a problem as we don't save

            return cart.CartItems.Values.ToList();
        }
        public List<int> GetAllCartitemsIds()
        {
            var cart = GetCart();
            return cart.CartItems.Keys.ToList();
        }
        public CartItem? GetCartItemById(int productId)
        {
            var cart = GetCart();
            var found = cart.CartItems.TryGetValue(productId, out var item);
            return item; // item is null if not found
        }

        public AddToCartResult AddToCart(int productId, int quantity = 1)
        {
            if (quantity <= 0)
                quantity = 1;
            var cart = GetCart();
            var product = _productService.GetProductById(productId);

            if (product == null)
                return AddToCartResult.ProductNotFound;

            if (quantity <= 0)
                quantity = 1;

            if (cart.CartItems.ContainsKey(productId))
            {
                var existing = cart.CartItems[productId];

                if (existing.Quantity + quantity > product.StockQuantity)
                    return AddToCartResult.OutOfStock;

                existing.Quantity += quantity;
                existing.LineTotal = existing.Quantity * existing.UnitPriceAtPurchase;
            }
            else
            {
                if (product.StockQuantity <= 0 || quantity > product.StockQuantity)
                    return AddToCartResult.OutOfStock;

                var item = new CartItem
                {
                    itemId = productId,
                    Quantity = quantity,
                    ProductName = product.ProductName,
                    UnitPriceAtPurchase = product.CurrentPrice,
                    StockStatus = StockStatus.InStock,
                    LineTotal = product.CurrentPrice * quantity,
                    DeliveryDate = DateTime.Now.AddDays(3).ToString("dd-MMM-yyyy"),
                    ImageUrl = product.ImageUrl,
                    AvailableStock = product.StockQuantity
                };

                cart.CartItems[productId] = item;
            }

            SaveCart(cart);
            return AddToCartResult.Success;
        }

        public bool RemoveItemFromCart(int itemId) 
        {
            // get cart in which the item exist
            var cart = GetCart();
            // remove it 
            bool removed = cart.CartItems.Remove(itemId);

            if (removed) SaveCart(cart);
           
            return removed; // false if key not found
        }

        public UpdateQuantityResult UpdateItemQuantity(int itemId , int qty)
        {
            var cart = GetCart();
            var ProductDB = _productService.GetProductById(itemId);
            if (ProductDB == null) return UpdateQuantityResult.ProductNotFound;            
            
            if (qty <= 0 || qty > ProductDB.StockQuantity)
            
            {            
                return UpdateQuantityResult.UnAvailableQuantity;
            }

            if (!cart.CartItems.TryGetValue(itemId, out var itemToBeUpdated))
            {
                return UpdateQuantityResult.ProductNotFound;
            }
           
            itemToBeUpdated.Quantity = qty;
            itemToBeUpdated.LineTotal = qty * itemToBeUpdated.UnitPriceAtPurchase;
            SaveCart(cart);
            return UpdateQuantityResult.Success;
        }
        public void RemoveCartCookie()
        {
            var session = _httpContextAccessor.HttpContext!.Session;
            session.Remove("Cart");
        }

    }
}
