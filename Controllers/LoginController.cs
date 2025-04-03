using JacoDemo.Models;
using Microsoft.AspNetCore.Mvc;

namespace JacoDemo.Controllers
{
    public class LoginController : Controller
    {
        // GET: /<controller>/
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult VerifyUser(LoginRequestModel loginRequestModel)
        {
            return View();
        }
    }
}
