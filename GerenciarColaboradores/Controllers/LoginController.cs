using Microsoft.AspNetCore.Mvc;

namespace GerenciarColaboradores.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }

}
