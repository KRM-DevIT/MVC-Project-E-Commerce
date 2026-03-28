using Microsoft.AspNetCore.Mvc;
using MiniECommerce.Models;

namespace MiniECommerce.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        public IActionResult Index()
        {
            var orders = _orderService.GetAllOrdersWithDetails(); 
            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStatus(int OrderId, OrderStatus status)
        {
            var result = _orderService.UpdateOrderStatus(OrderId, status);

            if (!result)
                TempData["ErrorMessage"] = $"Could not update status for order #{OrderId}.";
            else
                TempData["SuccessMessage"] = $"Order status updated successfully.";

            return RedirectToAction(nameof(Index));

        }
    }
}
