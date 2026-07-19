using MiniECommerce.DTO;

namespace MiniECommerce.Interfaces.Services
{
    public interface IOrderService
    {
        // === User Functions
        bool CreateOrder(Order order);
        Order? GetOrderDetails(int orderId);
        List<OrderDto> GetOrdersForUser(string userId);
        List<Order> GetAllOrders();
       
        // === Admin Functions

        bool UpdateOrderStatus(int orderId, OrderStatus status); 
        OrderDto? GetOrderWithDetails(int orderId);
        string GetUniqueOrderNumber();
        List<OrderDto> GetAllOrdersWithDetails();
    }
}
