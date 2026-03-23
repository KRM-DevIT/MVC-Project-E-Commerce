using Microsoft.AspNetCore.Http;
using MiniECommerce.Extensions;
using MiniECommerce.Models;
using MiniECommerce.ViewModels;
using System;
using System.Linq.Expressions;
using System.Transactions;

namespace MiniECommerce.Services
{
    public class CheckoutService
    {
        private readonly IOrderService _orderService;
        private readonly IOrderItemService _orderItemService;
        private readonly IProductService _productService;
        private readonly ApplicationDbContext _context;
        private readonly CartService _cartService;
        public CheckoutService(IOrderService orderService,IOrderItemService orderItemService,IProductService productService,ApplicationDbContext context, CartService cartService)
        {
            _orderService = orderService;
            _orderItemService = orderItemService;
            _productService = productService;
            _context = context;
            _cartService = cartService;
        }

       public CheckoutResult CheckOrderOut(CheckoutVM VM)
        {
            using(var scope = new TransactionScope())
            {
                try
                {
                    // 1- Validate Stock Quantity For each item in cart with its corresponding in DB
                    List<int> IDs = _cartService.GetAllCartitemsIds();
                    var DBProducts = _productService.GetProductsByIDs(IDs);
                    List<CartItem> cartItems = _cartService.GetAllCartItems();
                    foreach (CartItem cartItem in cartItems)
                    {
                        var DBItem = DBProducts.Find(p => p.ProductId == cartItem.itemId);
                        
                        if (DBItem!.StockQuantity < cartItem.Quantity)
                        {
                            return new CheckoutResult { Success = false, Message = "Insufficient stock" };
                        }
                    }
                     
                    // 2- Place Order in DB

                    var order = new Order
                    {
                        ApplicationUserId = VM.UserId,
                        ShippingAddressId = VM.ShippingAddressId,
                        Status = OrderStatus.Placed,
                        TotalAmount = _cartService.GetAllCartItems().Sum(i => i.LineTotal),
                        OrderDate = DateTime.Now,
                        OrderNumber = _orderService.GetUniqueOrderNumber()
                    };

                    var Created = _orderService.CreateOrder(order);
                    
                    if (!Created)
                    {
                        return new CheckoutResult { Success = false, Message = "Order Couldn't be placed Properly" };
                    }

                    // 3- Place Order Items in DB and Reduce Stock Quantity of each Product

                    foreach (var item in _cartService.GetAllCartItems())
                    {
                        var OrderItem = new OrderItem()
                        {
                            OrderId = order.OrderId,
                            Quantity = item.Quantity,
                            ProductId = item.itemId,
                            UnitPriceAtPurchase = item.UnitPriceAtPurchase,
                            LineTotal = item.LineTotal
                        };
                        
                        var created = _orderItemService.CreateOrderItem(OrderItem);
                        
                        if (created)
                        {
                            var product = _productService.GetProductById(item.itemId);
                            if (product != null)
                                product.StockQuantity -= item.Quantity;
                        }

                        else
                        {
                            return new CheckoutResult { Success = false, Message = "We couldn't Place All orderItems Checkout RolledBack" };
                        }
                    }

                    scope.Complete();

                    _cartService.RemoveCartCookie();

                    return new CheckoutResult { Success = true, Message = "Order Placed Successfully" };
                }

                catch
                {
                    return new CheckoutResult { Success = false, Message = "Error Processing Your Order" };
                }
            }
        }     

    }
}
