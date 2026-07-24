using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniECommerce.Areas.Customer.ViewModels.Catalog;
using MiniECommerce.Models;
using MiniECommerce.Services;

namespace MiniECommerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CatalogController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly CartService _cartService;
        public CatalogController(ICategoryService categoryService, IProductService productService , CartService cartService)
        {
            _categoryService = categoryService;
            _productService = productService;
            _cartService = cartService; 
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index(int pageNumber = 1, int pageSize = 10)
        {
            // Get Category List
            // Get Product List paginated
            var TotalCount = _productService.GetProductCount(activeOnly: true);
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

            var categories = _categoryService.GetCategoriesWithProducts();
            var products = _productService.GetProductsWithPagination(pageNumber, pageSize, activeOnly: true);
            var model = new CatalogViewModel
            {
                Categories = categories,
                Products = products,
                CurrentPage = pageNumber,
                TotalCount = TotalCount,
                PageSize = pageSize,
                CartProductIds = GetCustomerCartProductIds()
            };
            return View(nameof(Index),model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            var product = _productService.GetProductById(id);

            if (product == null || !product.IsActive)
            {
                return RedirectToAction(
                    "NotFoundPage",
                    "Error",
                    new { area = "" });
            }

            return View(product);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ProductsByCategory(List<int> Selectedcategories, int pageNumber = 1, int pageSize = 10)
        {
            if(Selectedcategories == null || Selectedcategories.Count == 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var products = _productService.GetProductsByCategoryWithPagination(Selectedcategories, pageNumber, pageSize);
            var TotalCount = _productService.GetProductsByCategoryCount(Selectedcategories);

            var categories = _categoryService.GetCategoriesWithProducts();
            var model = new CatalogViewModel
            {
                Categories = categories,
                Products = products,
                CurrentPage = pageNumber,
                TotalCount = TotalCount,
                PageSize = pageSize,
                SelectedCategories = Selectedcategories,
                CartProductIds = GetCustomerCartProductIds()
            };

            return View(nameof(Index), model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Search(List<int> Selectedcategories,string query, int pageNumber = 1, int pageSize = 10)
        {
            // validate query aren't empty
            if(string.IsNullOrEmpty(query))
            {
                return RedirectToAction(nameof(Index));
            }

            var products   =   _productService.SearchProductsWithPagination(Selectedcategories,query, pageNumber, pageSize);
            var TotalCount = _productService.GetSearchProductCount(Selectedcategories,query);
            var categories = _categoryService.GetCategoriesWithProducts();
            var model = new CatalogViewModel
            {
                Categories = categories,
                Products = products,
                CurrentPage = pageNumber,
                TotalCount = TotalCount,
                PageSize = pageSize,
                SelectedCategories = Selectedcategories,
                CartProductIds = GetCustomerCartProductIds()
            };

            return View(nameof(Index), model);
        }

        private List<int> GetCustomerCartProductIds()
        {
            return User.IsInRole("Customer")
                ? _cartService.GetAllCartitemsIds()
                : new List<int>();
        }

    }
}
