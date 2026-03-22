
namespace MiniECommerce.Repositories
{
    public class OrderRepository : Repository<Order> , IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public bool CheckUniquness(string orderNumber)
        {
          return _context.Orders.Any(o => o.OrderNumber == orderNumber);
        }

        public List<Order> GetOrdersForUser(string UserId)
        {
            return _context.Orders
                            .Where(o => o.ApplicationUserId == UserId).ToList();
        }
    }
}
