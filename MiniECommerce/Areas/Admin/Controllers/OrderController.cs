using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniECommerce.DTO;
using MiniECommerce.Interfaces.Repositories;
using MiniECommerce.Models;

namespace MiniECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private IUnitOfWork _unitOfWork;
        public OrderController(IOrderService orderService, IUnitOfWork unitOfWork)
        {
            _orderService = orderService;
            _unitOfWork = unitOfWork;
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
            bool result = _orderService.UpdateOrderStatus(OrderId, status);

            if (!result)
            {
                TempData["ErrorMessage"] = $"Could not update status for order #{OrderId}.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _unitOfWork.SaveChanges();

                TempData["SuccessMessage"] = "Order status updated successfully.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = $"Could not update status for order #{OrderId}.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
