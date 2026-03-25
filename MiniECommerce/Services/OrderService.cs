namespace MiniECommerce.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;

        public OrderService(IOrderRepository repository)
        {
            _repository = repository;
        }

        public bool CreateOrder(Order order)
        {
            bool orderExists= _repository.CheckUniquness(order.OrderNumber);
            if(orderExists) { return false;}

            _repository.Insert(order);
            _repository.Save();
            return true;
        }


        public List<Order> GetAllOrders()
        {
            return _repository.GetAll();
        }

        public bool UpdateOrderStatus(int orderId, OrderStatus status)
        {
            var order = _repository.GetById(orderId);
            if(order == null) { return false; }
            order.Status = status;
            _repository.Update(order);
            _repository.Save();
            return true;
        }
        public Order? GetOrderDetails(int orderId)
        {
            var order = _repository.GetById(orderId);
            if (order == null) return null;
            else
            {
                return order;
            }
        }

        public List<Order> GetOrdersForUser(string userId)
        {
           return _repository.GetOrdersForUser(userId);
        }

        public string GetUniqueOrderNumber()
        {
            return "ORD" + Guid.NewGuid().ToString().Substring(2, 10);
        }
    }
}
