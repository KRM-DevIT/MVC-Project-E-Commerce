
namespace MiniECommerce.Interfaces.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IOrderItemRepository _repository;

        public OrderItemService(IOrderItemRepository repository)
        {
            _repository = repository;
        }

        public bool CreateOrderItem(OrderItem item)
        {
            try
            {
                _repository.Insert(item);
                _repository.Save();
                return true;
            }
            catch {

                    return false;
                  }

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
