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
            var allOrders   = _orderService.GetAllOrders();
            var allCustomers = await _userManager.GetUsersInRoleAsync("Customer");

            // ── Stat cards ────────────────────────────────────────────
            var vm = new DashboardViewModel
            {
                TotalProducts   = allProducts.Count,
                ActiveProducts  = allProducts.Count(p => p.IsActive),
                TotalOrders     = allOrders.Count,
                TotalCustomers  = allCustomers.Count,
                PendingOrders   = allOrders.Count(o => o.Status == OrderStatus.Placed),
                

                // ── Recent orders table (last 5) ──────────────────────
                RecentOrders = allOrders
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .Select(o => new RecentOrderRow
                    {
                        OrderId      = o.OrderId,
                        OrderNumber  = o.OrderNumber,
                        CustomerName = o.ApplicationUser?.FullName ?? "—",
                        TotalAmount  = o.TotalAmount,
                        Status       = o.Status,
                        OrderDate    = o.OrderDate
                    })
                    .ToList(),

                // ── Low stock table (stock < threshold) ───────────────
                LowStockProducts = allProducts
                    .Where(p => p.StockQuantity < LowStockThreshold)
                    .OrderBy(p => p.StockQuantity)
                    .Select(p => new LowStockRow
                    {
                        ProductId     = p.ProductId,
                        ProductName   = p.ProductName,
                        StockQuantity = p.StockQuantity,
                        CategoryName  = p.Category?.CategoryName
                    })
                    .ToList()
            };

            return View(vm);
        }
    }
}
