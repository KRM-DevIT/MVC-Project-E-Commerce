using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MiniECommerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "Customer")]
    public class CartController : Controller
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        public IActionResult Index()
        {
            var CartItems = _cartService.GetAllCartItems();
            return View(CartItems);
        }

        // why the function add shows json result while the others don't ?
        // Edit it to show the same behavior as the others
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(int productId )
        {
            _cartService.AddToCart(productId);
            var count = _cartService.GetAllCartItems().Count;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateQuantity(int itemId, int qty)
        {
            var result = _cartService.UpdateItemQuantity(itemId, qty);
            return Json(new { success = result == UpdateQuantityResult.Success });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int itemId)
        {
            _cartService.RemoveItemFromCart(itemId);
            var count = _cartService.GetAllCartItems().Count;
            return Json(new { success = true, count });
        }
    }
}
