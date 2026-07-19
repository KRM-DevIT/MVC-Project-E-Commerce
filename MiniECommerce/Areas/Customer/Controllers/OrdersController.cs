using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MiniECommerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "Customer")]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var orders = _orderService.GetOrdersForUser(userId);

            return View(orders);
        }

        [HttpGet]
        public IActionResult Details(int id) 
        {
            var order = _orderService.GetOrderWithDetails(id);
            
            return View(order);
        }
    }
}
