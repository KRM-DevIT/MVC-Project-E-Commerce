using MiniECommerce.DTO;

namespace MiniECommerce.Interfaces.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        bool CheckUniquness(string orderNumber);

        List<Order> GetOrdersForUser(string userId);

        List<OrderDto> GetAllOrdersWithDetails();

        OrderDto? GetByIdWithDetails(int orderId);
    }
}
