using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniECommerce.Areas.Customer.ViewModels.Catalog;
using MiniECommerce.Models;

namespace MiniECommerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "Customer")]
    public class CatalogController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;

        public CatalogController(ICategoryService categoryService, IProductService productService)
        {
            _categoryService = categoryService;
            _productService = productService;
        }
        [HttpGet]
        public IActionResult Index(int pageNumber = 1, int pageSize = 10)
        {
            // Get Category List
            // Get Product List paginated
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

            var categories = _categoryService.GetCategoriesWithProducts();
            var products = _productService.GetProductsWithPagination(pageNumber, pageSize);
            var model = new CatalogViewModel
            {
                Categories = categories,
                Products = products,
                CurrentPage = pageNumber,
                TotalCount = TotalCount,
                PageSize = pageSize
            };
            return View(nameof(Index),model);
        }

        public IActionResult Details(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
                return RedirectToAction("NotFoundPage", "Error", new { area = "" });
            return View(product);
        }

        [HttpGet]
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
                SelectedCategories = Selectedcategories
            };

            return View(nameof(Index), model);
        }

        //public IActionResult Search(string query, int pageNumber = 1, int pageSize = 10)
        //{
        //    var products = _productService.SearchProductsWithPagination(query, pageNumber, pageSize);
        //    var totalCount = _productService.GetSearchProductCount(query);
        //    var model = new CatalogViewModel
        //    {
        //        Categories = _categoryService.GetAllCategories(),
        //        Products = products
        //    };
        //    ViewBag.CurrentPage = pageNumber;
        //    ViewBag.TotalCount = totalCount;
        //    ViewBag.PageSize = pageSize;
        //    ViewBag.Query = query;
        //    return View(nameof(Index), model);
        //}

    }
}
