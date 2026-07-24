using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniECommerce.Areas.Admin.ViewModels.ProductViewModels;
using MiniECommerce.Interfaces.Repositories;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MiniECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private IUnitOfWork _unitOfWork;
        public ProductController(IProductService productService, ICategoryService categoryService,IUnitOfWork unitOfWork)
        {
            _productService = productService;
            _categoryService = categoryService;
            _unitOfWork = unitOfWork;
        }

        //================= INDEX =======================================
        [HttpGet]
        public IActionResult Index(int pageNumber = 1, int pageSize = 10)
        {
            var TotalCount = _productService.GetProductCount();
            int totalPages = (int)Math.Ceiling(TotalCount / (double)pageSize);
            if (pageNumber > totalPages)
            {
                pageNumber = totalPages;
            }

            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            var products = _productService.GetProductsWithPagination(pageNumber, pageSize);

            var ProductListVM = new ProductListViewModel
            {
                Products = products,
                CurrentPage = pageNumber,
                TotalCount = TotalCount,
                PageSize = pageSize
            };

            return View(nameof(Index), ProductListVM);
        }


        //========================Details=================================
        [HttpGet]
        public IActionResult Details(int id)
        {
            var product = _productService.GetProductWithCategory(id);
            if (product == null)
                return RedirectToAction("NotFoundPage", "Error", new { area = "" });
            return View(product);
        }


        // ===================CREATE GET +=======================================
        [HttpGet]
        public IActionResult Create()
        {
            var model = new ProductCreateViewModel()
            {
                categories = _categoryService.CategoryDropDownList()
            };

            return View(model);
        }

        //===========================CREATE POST +=====================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(ProductCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.categories = _categoryService.CategoryDropDownList();
                return View(model);
            }

            // --------------------------Handle image upload-----------------
            string? imagePath = null;

            if (model.ImageFile != null)
            {
                var (success, filePath, error) = await _productService.SaveImageAsync(model.ImageFile);

                if (!success)
                {
                    ModelState.AddModelError(nameof(model.ImageFile), error!);
                    model.categories = _categoryService.CategoryDropDownList();
                    return View(model);
                }

                imagePath = filePath;
            }

            //============== ADD TO DB=====================
            var productDB = new Product
            {
                ProductName = model.ProductName,
                CurrentPrice = model.CurrentPrice,
                Description = model.Description,
                ImageUrl = imagePath,
                StockQuantity = model.StockQuantity,
                IsActive = model.IsActive,
                CategoryId = model.CategoryId,
            };

            var categoryName = _categoryService
                .GetCategoryById(model.CategoryId)!.CategoryName;

            productDB.SKU = _productService.GenerateUniqueSKU(
                model.ProductName,
                categoryName);

            bool result = _productService.CreateProduct(productDB);

            if (!result)
            {
                ModelState.AddModelError(string.Empty, "Product SKU already exists.");
                model.categories = _categoryService.CategoryDropDownList();
                return View(model);
            }

            try
            {
                _unitOfWork.SaveChanges();

                TempData["SuccessMessage"] = "Product Created Successfully";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Error occurred while saving product.");
                model.categories = _categoryService.CategoryDropDownList();
                return View(model);
            }
        }

        // ── EDIT GET ──────────────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
                return RedirectToAction("NotFoundPage", "Error", new { area = "" });

            var model = new ProductEditViewModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                CurrentPrice = product.CurrentPrice,
                Description = product.Description,
                SKU = product.SKU,
                ExistingImageUrl = product.ImageUrl,
                StockQuantity = product.StockQuantity,
                IsActive = product.IsActive,
                CategoryId = product.CategoryId,
                categories = _categoryService.CategoryDropDownList()
            };

            return View(model);
        }

        // ── EDIT POST ─────────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductEditViewModel model)
        {
            if (id != model.ProductId)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                model.categories = _categoryService.CategoryDropDownList();
                return View(model);
            }

            var product = _productService.GetProductById(id);

            if (product == null)
                return RedirectToAction("NotFoundPage", "Error", new { area = "" });

            string? oldImagePath = product.ImageUrl;

            // Handle image upload
            if (model.ImageFile != null)
            {
                var (success, filePath, error) = await _productService.SaveImageAsync(model.ImageFile);

                if (!success)
                {
                    ModelState.AddModelError(nameof(model.ImageFile), error!);
                    model.categories = _categoryService.CategoryDropDownList();
                    model.ExistingImageUrl = product.ImageUrl;
                    return View(model);
                }

                product.ImageUrl = filePath;
            }

            product.ProductName = model.ProductName;
            product.CurrentPrice = model.CurrentPrice;
            product.Description = model.Description;
            product.StockQuantity = model.StockQuantity;
            product.IsActive = model.IsActive;
            product.CategoryId = model.CategoryId;

            try
            {
                _productService.UpdateProduct(product);

                _unitOfWork.SaveChanges();

                if (model.ImageFile != null)
                    _productService.DeleteImage(oldImagePath); // Delete old image after DB update succeeds

                TempData["SuccessMessage"] = "Product updated successfully.";

                return RedirectToAction(nameof(Details), new { id = product.ProductId });
            }
            catch (Exception)
            {
                // Delete the newly uploaded image if DB update failed
                if (model.ImageFile != null)
                {
                    _productService.DeleteImage(product.ImageUrl);
                    product.ImageUrl = oldImagePath;
                }

                ModelState.AddModelError(string.Empty, "An error occurred while updating the product.");
                model.categories = _categoryService.CategoryDropDownList();
                model.ExistingImageUrl = oldImagePath;

                return View(model);
            }
        }

        // ── DELETE POST ───────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var product = _productService.GetProductById(id);

            if (product == null)
            {
                return RedirectToAction(
                    "NotFoundPage",
                    "Error",
                    new { area = "" });
            }

            var productName = product.ProductName;
            var imagePath = product.ImageUrl;

            bool result = _productService.DeleteProduct(id);

            if (!result)
            {
                TempData["ErrorMessage"] = "Product not found.";
                return RedirectToAction(nameof(Details), new { id });
            }

            try
            {
                _unitOfWork.SaveChanges();
            }

            catch (Exception)
            {
                TempData["ErrorMessage"] =
                    "The product could not be deleted. It may be referenced by an existing order.";

                return RedirectToAction(nameof(Details), new { id });
            }

            try
            {
                _productService.DeleteImage(imagePath);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] =
                    $"'{productName}' was deleted, but its old image could not be removed.";

                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] =
                $"'{productName}' was deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

    }
}



