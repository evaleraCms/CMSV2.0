using Microsoft.AspNetCore.Mvc;

namespace CMS.RestApi.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
