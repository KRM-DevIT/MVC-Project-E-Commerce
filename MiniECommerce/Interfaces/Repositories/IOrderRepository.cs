namespace MiniECommerce.Interfaces.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        bool CheckUniquness(string orderNumber);

        List<Order> GetOrdersForUser(string userId);
    }
}
