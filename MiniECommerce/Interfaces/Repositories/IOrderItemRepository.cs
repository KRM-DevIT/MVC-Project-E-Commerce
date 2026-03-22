namespace MiniECommerce.Interfaces.Repositories
{
    public interface IOrderItemRepository : IRepository<OrderItem>
    {
        List<OrderItem> OrderItemsPerOrder(int orderId);

        List<OrderItem> OrderItemsPerProduct(int productId);
    }
}
