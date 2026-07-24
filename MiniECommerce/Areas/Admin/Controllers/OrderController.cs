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
        public IActionResult UpdateStatus(int orderId, OrderStatus status)
        {
            bool result = _orderService.UpdateOrderStatus(orderId, status);

            if (!result)
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"Could not update status for order #{orderId}."
                });
            }

            try
            {
                _unitOfWork.SaveChanges();

                return Ok(new
                {
                    success = true,
                    message = "Order status updated successfully.",
                    status = status.ToString()
                });
            }
            catch
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Could not update status for order #{orderId}."
                });
            }
        }
    }
}
