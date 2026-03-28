
using Microsoft.EntityFrameworkCore;
using MiniECommerce.DTO;
using System.Linq.Expressions;

namespace MiniECommerce.Repositories
{
    public class OrderRepository : Repository<Order> , IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public bool CheckUniquness(string orderNumber)
        {
          return _context.Orders.Any(o => o.OrderNumber == orderNumber);
        }

        public List<OrderDto> GetAllOrdersWithDetails()
        {
            return _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .Select(ToDto)     
                .ToList();
        }

        public OrderDto? GetByIdWithDetails(int orderId)
        {
            return _context.Orders
                .Where(o => o.OrderId == orderId)   
                .Select(ToDto)                      
                .FirstOrDefault();
        }

        public List<Order> GetOrdersForUser(string UserId)
        {
            return _context.Orders
                            .Where(o => o.ApplicationUserId == UserId).ToList();
        }

        // This field helps to DRY principle
        private static readonly Expression<Func<Order, OrderDto>> ToDto = o => new OrderDto
        {
            OrderId = o.OrderId,
            OrderNumber = o.OrderNumber,
            OrderDate = o.OrderDate,
            CustomerName = o.ApplicationUser!.FirstName + " " + o.ApplicationUser.LastName,
            CustomerEmail = o.ApplicationUser.Email!,
            TotalAmount = o.TotalAmount,
            Status = o.Status,
            ShippingAddress = o.Address == null ? null :
                        $"{o.Address.Street}, {o.Address.City}, {o.Address.Country} {o.Address.Zip}",
            Items = o.OrderItems.Select(oi => new OrderItemDto
            {
                ProductName = oi.Product!.ProductName ?? "--",
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPriceAtPurchase,
                LineTotal = oi.LineTotal
            }).ToList()
        };

    }


}


// Below are unOptimized Queries that return all the columns from DB ( BAD )
//public List<Order> GetAllOrdersWithDetails()
//{
//    return _context.Orders
//        .Include(o => o.ApplicationUser)
//        .Include(o => o.OrderItems)
//            .ThenInclude(oi => oi.Product)
//        .Include(o => o.Address)
//        .OrderByDescending(o => o.OrderDate)
//        .ToList();
//}

//// Single order with all details for the details/expand view
//public Order? GetByIdWithDetails(int orderId)
//{
//    return _context.Orders
//        .Include(o => o.ApplicationUser)
//        .Include(o => o.OrderItems)
//            .ThenInclude(oi => oi.Product)
//        .Include(o => o.Address)
//        .FirstOrDefault(o => o.OrderId == orderId);
//}