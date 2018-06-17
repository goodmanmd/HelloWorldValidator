using Microsoft.AspNetCore.Mvc;

namespace HelloWorldValidator.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
