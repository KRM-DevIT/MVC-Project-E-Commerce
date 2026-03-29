using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniECommerce.DTO;
using MiniECommerce.Models;

namespace MiniECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
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
            var orderList = new OrderList();
            orderList.Orders = orders;
            return View(orderList);
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
