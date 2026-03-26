using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniECommerce.Areas.Admin.ViewModels.ProductViewModels;

namespace MiniECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet]
        public IActionResult Index(int pageNumber = 1,int pageSize=10)
        {
            var products = _productService.GetProductsWithPagination(pageNumber,pageSize);
            var TotalCount = _productService.GetProductCount();

            var ProductListVM = new ProductListViewModel
            {
                Products= products,
                CurrentPage = pageNumber,
                TotalCount = TotalCount,
                PageSize = pageSize
            };



            return View(nameof(Index),ProductListVM);
        }

        [HttpGet]
        public IActionResult Create() 
        { 
            return View();
        }
    }
}
