using Microsoft.AspNetCore.Mvc;

namespace MiniECommerce.Controllers
{
    

    public class ErrorController : Controller
    {

        public IActionResult NotFoundPage()
        {
            Response.StatusCode = 404; 
            return View(); 
        }
    }
}
