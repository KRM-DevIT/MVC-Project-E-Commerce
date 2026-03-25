using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace MiniECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RoleController(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }


        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Action = "Create";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var role = new ApplicationRole
            {
                Name = model.RoleName,
                RoleDescription = model.RoleDescription,
                RoleImageURL = model.RoleImageURL
            };

            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Role created successfully!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(Id);

            if (role == null)
            {
                return RedirectToAction("NotFoundPage", "Error", new { area = "" });
            }

            var model = new RoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name!,
                RoleDescription = role.RoleDescription,
                RoleImageURL = role.RoleImageURL
            };
            ViewBag.RoleId = role.Id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string Id, RoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var role = await _roleManager.FindByIdAsync(Id);

            if (role == null)
            {
                return RedirectToAction("NotFoundPage", "Error", new { area = "" });
            }

            role.Name = model.RoleName;
            role.RoleDescription = model.RoleDescription;
            role.RoleImageURL = model.RoleImageURL;

            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return RedirectToAction("NotFoundPage", "Error", new { area = "" });
            }

            var role = await _roleManager.FindByIdAsync(Id);

            if (role == null)
            {
                return RedirectToAction("NotFoundPage", "Error", new { area = "" });
            }

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Role '{role.Name}' deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = $"Error deleting role '{role.Name}'.";
            return RedirectToAction(nameof(Index));
        }
    }
}