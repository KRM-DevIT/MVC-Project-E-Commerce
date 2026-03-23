namespace MiniECommerce.Interfaces.Services
{
    public interface IOrderService
    {
        Order? GetOrderDetails(int orderId);
        List<Order> GetOrdersForUser(string userId);
        List<Order> GetAllOrders();
        bool UpdateOrderStatus(int orderId, OrderStatus status); 
        bool CreateOrder(Order order);

        string GetUniqueOrderNumber();
    }
}
