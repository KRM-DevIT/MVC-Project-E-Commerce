using Microsoft.AspNetCore.Mvc;

namespace MiniECommerce.Controllers
{
    

    public class ErrorController : Controller
    {

        public IActionResult NotFoundPage(string message="Error The page Requested Not Found ")
        {
            Response.StatusCode = 404;
            ViewBag.message = message;
            return View(); 
        }
    }
}
