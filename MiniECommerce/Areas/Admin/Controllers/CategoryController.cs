using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniECommerce.Areas.Admin.ViewModels.CategoryViewModels;

namespace MiniECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(ICategoryService categoryService , IUnitOfWork unitOfWork)
        {
            _categoryService = categoryService; 
            _unitOfWork = unitOfWork;
        }
        
        public IActionResult Index()
        {
            var model = _categoryService.GetAllCategoriesWithParent(); // no inner join with it self -> no parent is loaded     
            return View(nameof(Index),model);
        }

        [HttpGet]
        public IActionResult Create()
        {
          var CategoryList = _categoryService.CategoryDropDownList();
          var model = new CategoryViewModel { Categories = CategoryList };
          return View(nameof(Create),model); // to choose parent from DDList 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = _categoryService.CategoryDropDownList();
                return View(nameof(Create), model);
            }

            var category = new Category
            {
                CategoryName = model.CategoryName,
                ParentCategoryId = model.ParentCategoryId
            };

            bool result = _categoryService.CreateNewCategory(category);

            if (!result)
            {
                model.Categories = _categoryService.CategoryDropDownList();
                ModelState.AddModelError(nameof(model.CategoryName), "Not Unique Name");
                return View(nameof(Create), model);
            }

            try
            {
                _unitOfWork.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                model.Categories = _categoryService.CategoryDropDownList();
                ModelState.AddModelError("", "An error occurred while saving the category.");

                return View(nameof(Create), model);
            }
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var category = _categoryService.GetCategoryByIdWithParent(id);

            if (category == null)
                return RedirectToAction("NotFoundPage", "Error",new { area = "", message = "Can't Find Category Details" });
            return View(category);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _categoryService.GetCategoryById(id);

            if (category == null)
                return RedirectToAction("NotFoundPage", "Error", new { area = "" , message="Couldn't Find Category To Edit May be Deleted"});


            var model = new CategoryViewModel
            {
                CategoryName = category.CategoryName,
                ParentCategoryId = category.ParentCategoryId
            };

            model.Categories = _categoryService.CategoryDropDownList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, CategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = _categoryService.CategoryDropDownList();
                return View(model);
            }

            var category = _categoryService.GetCategoryById(id);

            if (category == null)
            {
                return RedirectToAction(
                    "NotFoundPage",
                    "Error",
                    new
                    {
                        area = "",
                        message = "Couldn't Find Category To Edit May be Deleted"
                    });
            }

            category.CategoryName = model.CategoryName;
            category.ParentCategoryId = model.ParentCategoryId;

            try
            {
                _categoryService.UpdateCategory(category);

                _unitOfWork.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                model.Categories = _categoryService.CategoryDropDownList();
                ModelState.AddModelError("", "Error updating category.");

                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var category = _categoryService.GetCategoryById(id);

            if (category == null)
                return RedirectToAction("NotFoundPage", "Error", new { area = "",message="Couldn't Find Category may be deleted Already" });


            return View(category);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            bool result = _categoryService.DeleteCategory(id);

            if (!result)
                return BadRequest();

            try
            {
                _unitOfWork.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
