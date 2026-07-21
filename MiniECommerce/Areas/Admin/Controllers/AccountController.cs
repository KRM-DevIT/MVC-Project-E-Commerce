using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MiniECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly RoleManager<ApplicationRole> _roleManager;
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        #region Registration

        [HttpGet]
        [Authorize(Roles ="Admin")]
        public IActionResult AdminRegister()
        {
            return View(nameof(AdminRegister)); // only if view and action name are similer
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> AdminRegister(AdminRegisterViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = $"{model.FirstName}_{model.LastName}"
            };


            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Admin");

                return RedirectToAction(nameof(AdminLogin)); // regiter success go to login
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(nameof(AdminRegister),model);
        }

        #endregion 

        #region Login

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AdminLogin(string? returnUrl = null)
        {
            // If already authenticated, no need to show login page
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

            }

            // Store returnUrl in ViewBag so the form can post it back
            ViewData["ReturnUrl"] = returnUrl;
            return View(nameof(AdminLogin));
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminLogin(AdminLoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login.");
                return View(model);
            }

            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                ModelState.AddModelError("", "You are not authorized.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
            user,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: true); // configuration added in the program.cs

            if (result.Succeeded)
            {

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

            }

            if (result.IsNotAllowed)
            {
                // Email not confirmed or account disabled
                ModelState.AddModelError(string.Empty, "Account not allowed to sign in.");
            }
            else
            {
                // Generic error for wrong password
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(nameof(AdminLogin), model);
        }

        #endregion

        #region Logout

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> AdminLogout()
            {
                 await _signInManager.SignOutAsync();
                 return RedirectToAction("Index", "Home", new { area = "" });
            }

        #endregion

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

 