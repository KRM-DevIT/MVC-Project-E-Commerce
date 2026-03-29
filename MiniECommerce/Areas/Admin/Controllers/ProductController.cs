using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniECommerce.Areas.Admin.ViewModels.ProductViewModels;
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
        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        //================= INDEX =======================================
        [HttpGet]
        public IActionResult Index(int pageNumber = 1, int pageSize = 10)
        {
            var TotalCount = _productService.GetProductCount();
            int totalPages = (int)Math.Ceiling(TotalCount / (double)pageSize);
            // server - side validation
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
            var product = _productService.GetProductById(id);
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
            var productDB = new Product()
            {
                ProductName = model.ProductName,
                CurrentPrice = model.CurrentPrice,
                Description = model.Description,
                ImageUrl = imagePath,
                StockQuantity = model.StockQuantity,
                IsActive = model.IsActive,
                CategoryId = model.CategoryId,
            };

            var categoryName = _categoryService.GetCategoryById(model.CategoryId)!.CategoryName;
            productDB.SKU = _productService.GenerateUniqueSKU(model.ProductName, categoryName);

            var result = _productService.CreateProduct(productDB);
            //=======================================
            if (result)
            {
                TempData["SuccessMessage"] = "Product Created Succefully";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Error Occurred while saving Product");
            model.categories = _categoryService.CategoryDropDownList();
            return View(model);
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
            // Handle image — only replace if a new file was uploaded
            if (model.ImageFile != null)
            {
                var (success, filePath, error) = await _productService.SaveImageAsync(model.ImageFile);
                if (!success)
                {
                    ModelState.AddModelError(nameof(model.ImageFile), error!);
                    model.categories = _categoryService.CategoryDropDownList();
                    model.ExistingImageUrl = product.ImageUrl; // restore preview
                    return View(model);
                }

                product.ImageUrl = filePath; // update the ImagePath
            }

            product.ProductName = model.ProductName;
            product.CurrentPrice = model.CurrentPrice;
            product.Description = model.Description;
            product.StockQuantity = model.StockQuantity;
            product.IsActive = model.IsActive;
            product.CategoryId = model.CategoryId;

            var result = _productService.UpdateProduct(product);
            if (result)
            {
                if(model.ImageFile !=null) 
                    _productService.DeleteImage(product.ImageUrl); // will delete the old image from server

                TempData["SuccessMessage"] = "Product updated successfully.";
                return RedirectToAction(nameof(Details), new { id = product.ProductId });
            }

            if (model.ImageFile != null) // if db update failed will delete the new image from server
                _productService.DeleteImage(product.ImageUrl);

            ModelState.AddModelError(string.Empty, "An error occurred while updating the product.");
            model.categories = _categoryService.CategoryDropDownList();
            model.ExistingImageUrl = oldImagePath; // if update didn't go fine we restore the last image (no delete done)
            return View(model);
        }

        // ── DELETE POST ───────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
                return RedirectToAction("NotFoundPage", "Error", new { area = "" });

            _productService.DeleteImage(product.ImageUrl);

            var result = _productService.DeleteProduct(id);
            if (result)
            {
                TempData["SuccessMessage"] = $"'{product.ProductName}' was deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "An error occurred while deleting the product.";
            return RedirectToAction(nameof(Details), new { id });
        }

    }
}



