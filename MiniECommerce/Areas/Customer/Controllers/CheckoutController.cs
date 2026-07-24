using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniECommerce.Areas.Customer.ViewModels;
using MiniECommerce.Models.IdentityModels;
using MiniECommerce.Services;
using System.Security.Claims;

namespace MiniECommerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "Customer")]
    public class CheckoutController : Controller
    {
        private readonly CheckoutService _checkoutService;
        private readonly IAddressService _addressService;
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly CartService _cartService;

        public CheckoutController(
            CheckoutService checkoutService,
            IAddressService addressService,
            IOrderService orderService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            CartService cartService)
        {
            _checkoutService = checkoutService;
            _addressService = addressService;
            _orderService = orderService;
            _userManager = userManager;
            _signInManager = signInManager;
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cartItems = _cartService.GetAllCartItems();

            // Redirect to cart if empty
            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty. Please add items to continue.";
                return RedirectToAction("Index", "Cart");
            }

            // Get current user
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))

            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login", "Account");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get user addresses
            var addresses = _addressService.GetAddresses(userId);

            // Validate that user has at least one address
            if (!addresses.Any())
            {
                TempData["WarningMessage"] = "Please add a shipping address to continue.";
                return RedirectToAction("AddAddress", "Account");
            }

            var checkoutVM = new CheckoutVM
            {
                UserId = userId,
                UserName = $"{user.FirstName} {user.LastName}",
                UserEmail = user.Email!,
                Addresses = addresses,
                CartTotal = cartItems.Sum(i => i.LineTotal),
                Shipping = 5.99m,
                ShippingAddressId = addresses.FirstOrDefault(a => a.IsDefault)?.AddressId ?? 0,
                CartItems = cartItems
            };


            return View(checkoutVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessCheckout(ProcessCheckoutVM checkoutVM)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill in all required fields.";
                return RedirectToAction("Index");
            }

            // validate if shipping address ID isn't zero 
            if (checkoutVM.ShippingAddressId == 0)
            {
                TempData["ErrorMessage"] = "Please Choose Address to send the cargo to";
                return RedirectToAction("Index");
            }
            
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(UserId))
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login","Account");
            }

            var address = _addressService.GetAddressById(UserId, checkoutVM.ShippingAddressId);

            if (address == null)
            {
                TempData["ErrorMessage"] = "Invalid shipping address selected,choose another one or add one";
                return RedirectToAction("Index");
            }

            // Process checkout
            var result = await _checkoutService.CheckOrderOutAsync(UserId,checkoutVM);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Index");
            }

            TempData["SuccessMessage"] = "Order placed successfully!";
            return RedirectToAction("Confirmation", new { orderId = result.OrderId });
        }

        [HttpGet]
        public IActionResult Confirmation(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if(String.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            // Fetch order to display confirmation
            var order = _orderService.GetOrderForUser(userId,orderId);
            
            if (order == null || order.OrderId == 0)
            {
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction("Index", "Catalog");
            }

            return View(order);
        }
    }
}
