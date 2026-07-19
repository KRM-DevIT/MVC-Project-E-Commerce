namespace MiniECommerce.Interfaces.Services
{
    public interface IOrderItemService
    {
        List<OrderItem> GetAllOrderItemsPerProduct(int productId);
        List<OrderItem> GetAllOrderItemsPerOrder(int orderId);
        void CreateOrderItem(OrderItem item);

    }
}
