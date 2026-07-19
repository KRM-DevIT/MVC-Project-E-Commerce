using MiniECommerce.Areas.Customer.ViewModels;
using MiniECommerce.Extensions;
using MiniECommerce.Models;
using MiniECommerce.Interfaces.Repositories;

namespace MiniECommerce.Services
{
    public class CheckoutService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderService _orderService;
        private readonly IOrderItemService _orderItemService;
        private readonly IProductService _productService;
        private readonly CartService _cartService;

        public CheckoutService(
            IUnitOfWork unitOfWork,
            IOrderService orderService,
            IOrderItemService orderItemService,
            IProductService productService,
            CartService cartService)
        {
            _unitOfWork = unitOfWork;
            _orderService = orderService;
            _orderItemService = orderItemService;
            _productService = productService;
            _cartService = cartService;
        }

        public async Task<CheckoutResult> CheckOrderOutAsync(string UserId,ProcessCheckoutVM vm)
        {
            // =========================
            // Validate Cart
            // =========================

            List<CartItem> cartItems = _cartService.GetAllCartItems();

            List<int> productIds = cartItems
                .Select(x => x.itemId)
                .ToList();

            List<Product> dbProducts = _productService.GetProductsByIDs(productIds);

            foreach (CartItem cartItem in cartItems)
            {
                Product? product = dbProducts
                    .FirstOrDefault(p => p.ProductId == cartItem.itemId);

                if (product == null)
                {
                    return new CheckoutResult
                    {
                        Success = false,
                        Message = $"{cartItem.ProductName} is no longer available."
                    };
                }

                if (product.StockQuantity < cartItem.Quantity)
                {
                    return new CheckoutResult
                    {
                        Success = false,
                        Message = $"Insufficient stock for {product.ProductName}. Only {product.StockQuantity} available."
                    };
                }
            }

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {

                try
                {
                    // =========================
                    // Create Order
                    // =========================

                    Order order = new()
                    {
                        ApplicationUserId = UserId,
                        ShippingAddressId = vm.ShippingAddressId,
                        Status = OrderStatus.Placed,
                        TotalAmount = cartItems.Sum(x => x.LineTotal),
                        OrderDate = DateTime.Now,
                        OrderNumber = _orderService.GetUniqueOrderNumber()
                    };

                    _orderService.CreateOrder(order); // one call — EF tracks the whole graph from here

                    bool orderCreated = _orderService.CreateOrder(order);

                    if (!orderCreated)
                    {
                        await transaction.RollbackAsync();

                        return new CheckoutResult
                        {
                            Success = false,
                            Message = "Failed to create order."
                        };
                    }

                    // =========================
                    // Create Order Items
                    // Update Stock
                    // =========================

                    foreach (CartItem item in cartItems)
                    {
                        _orderItemService.CreateOrderItem(new OrderItem
                        {
                            Order = order,   // Relationship-Fixup
                            ProductId = item.itemId,
                            Quantity = item.Quantity,
                            UnitPriceAtPurchase = item.UnitPriceAtPurchase,
                            LineTotal = item.LineTotal
                        });

                        Product product = dbProducts
                            .First(p => p.ProductId == item.itemId);

                        product.StockQuantity -= item.Quantity;

                        _productService.UpdateProduct(product);
                    }
                    //======// Another Approch for the previous 2 steps========
                        //Order order = new()
                        //{
                        //    ApplicationUserId = UserId,
                        //    ShippingAddressId = vm.ShippingAddressId,
                        //    Status = OrderStatus.Placed,
                        //    TotalAmount = cartItems.Sum(x => x.LineTotal),
                        //    OrderDate = DateTime.Now,
                        //    OrderNumber = _orderService.GetUniqueOrderNumber(),
                        //    OrderItems = cartItems.Select(item => new OrderItem
                        //    {
                        //        ProductId = item.itemId,
                        //        Quantity = item.Quantity,
                        //        UnitPriceAtPurchase = item.UnitPriceAtPurchase,
                        //        LineTotal = item.LineTotal
                        //    }).ToList()
                        //};
                    //===================================
                    await Task.Delay(5000);

                    await _unitOfWork.SaveChangesAsync();

                    await transaction.CommitAsync();

                    _cartService.RemoveCartCookie();

                    return new CheckoutResult
                    {
                        Success = true,
                        Message = "Order placed successfully!",
                        OrderId = order.OrderId,
                        OrderNumber = order.OrderNumber
                    };

                }

                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    return new CheckoutResult
                    {
                        Success = false,
                        Message = $"Error processing your order: {ex.Message}"
                    };
                }
            }
        }
    }
}
