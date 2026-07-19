using MiniECommerce.DTO;

namespace MiniECommerce.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;

        public OrderService(IOrderRepository repository)
        {
            _repository = repository;
        }

        // ================ User-Services=======================
        public bool CreateOrder(Order order) //1
        {
            bool orderExists = _repository.CheckUniquness(order.OrderNumber);
            if (orderExists) return false;

            _repository.Insert(order);
            return true;
        }

        public List<Order> GetAllOrders()
        {
            return _repository.GetAll();
        }

        public Order? GetOrderDetails(int orderId)
        {
            return _repository.GetById(orderId);
        }

        public List<OrderDto> GetOrdersForUser(string userId)
        {
            return _repository.GetOrdersForUser(userId);
        }

        // =================== Admin Services======================

        public List<OrderDto> GetAllOrdersWithDetails()
        {
            return _repository.GetAllOrdersWithDetails();
        }
        public OrderDto? GetOrderWithDetails(int orderId)
        {
            return _repository.GetByIdWithDetails(orderId);
        }

        public bool UpdateOrderStatus(int orderId, OrderStatus status) //2
        {
            var order = _repository.GetById(orderId);
            if (order == null) return false;

            order.Status = status;
            _repository.Update(order);
            
            return true;
        }

        public string GetUniqueOrderNumber()
        {
            return "ORD" + Guid.NewGuid().ToString().Substring(2, 10);
        }
    }
}