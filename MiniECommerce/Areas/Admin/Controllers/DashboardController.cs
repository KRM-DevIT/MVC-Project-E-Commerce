using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniECommerce.Areas.Admin.ViewModels;
using MiniECommerce.Results;

namespace MiniECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly ICategoryService _categoryService;
        private readonly UserManager<ApplicationUser> _userManager;

        private const int LowStockThreshold = 5;

        public DashboardController(
            IProductService productService,
            IOrderService orderService,
            ICategoryService categoryService,
            UserManager<ApplicationUser> userManager)
        {
            _productService = productService;
            _orderService = orderService;
            _categoryService = categoryService;
            _userManager = userManager;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var allProducts = _productService.GetAllProducts();
            var allOrders = _orderService.GetAllOrders();

            var allCustomers =
                await _userManager.GetUsersInRoleAsync("Customer");

            var allCategories =
                _categoryService.GetAllCategories();

            // ApplicationUserId -> customer's full name
            var customerNames = allCustomers.ToDictionary(
                customer => customer.Id,
                customer => customer.FullName);

            // CategoryId -> category name
            var categoryNames = allCategories.ToDictionary(
                category => category.CategoryId,
                category => category.CategoryName);

            var vm = new DashboardViewModel
            {
                TotalProducts = allProducts.Count,

                ActiveProducts = allProducts.Count(
                    product => product.IsActive),

                TotalOrders = allOrders.Count,

                TotalCustomers = allCustomers.Count,

                PendingOrders = allOrders.Count(
                    order => order.Status == OrderStatus.Placed),

                RecentOrders = allOrders
                    .OrderByDescending(order => order.OrderDate)
                    .Take(5)
                    .Select(order => new RecentOrderRow
                    {
                        OrderId = order.OrderId,
                        OrderNumber = order.OrderNumber,

                        CustomerName = customerNames.GetValueOrDefault(
                            order.ApplicationUserId,
                            "Unknown customer"),

                        TotalAmount = order.TotalAmount,
                        Status = order.Status,
                        OrderDate = order.OrderDate
                    })
                    .ToList(),

                LowStockProducts = allProducts
                    .Where(product =>
                        product.StockQuantity < LowStockThreshold)
                    .OrderBy(product => product.StockQuantity)
                    .Select(product => new LowStockRow
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        StockQuantity = product.StockQuantity,

                        CategoryName = categoryNames.GetValueOrDefault(
                            product.CategoryId,
                            "Unknown category")
                    })
                    .ToList()
            };

            return View(vm);
        }
    }
}
