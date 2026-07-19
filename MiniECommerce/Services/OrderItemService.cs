namespace MiniECommerce.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IOrderItemRepository _repository;

        public OrderItemService(IOrderItemRepository repository)
        {
            _repository = repository;
        }

        public void CreateOrderItem(OrderItem item)
        {

                _repository.Insert(item);

        }

        public List<OrderItem> GetAllOrderItemsPerOrder(int orderId)
        {
           return _repository.OrderItemsPerOrder(orderId);
        }

        public List<OrderItem> GetAllOrderItemsPerProduct(int productId)
        {
            return _repository.OrderItemsPerProduct(productId);
        }
    }
}
