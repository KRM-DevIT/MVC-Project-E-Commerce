
namespace MiniECommerce.Repositories
{
    public class OrderItemRepository : Repository<OrderItem> , IOrderItemRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderItemRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public List<OrderItem> OrderItemsPerOrder(int orderId)
        {
            return _context.OrderItems.Where(o => o.OrderId == orderId).ToList();
        }

        public List<OrderItem> OrderItemsPerProduct(int productId)
        {
            return _context.OrderItems.Where(o => o.ProductId == productId).ToList();
        }

      
    }
}
