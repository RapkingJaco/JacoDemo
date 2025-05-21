using JacoDemo.Models;
using Microsoft.AspNetCore.Mvc;

namespace JacoDemo.Controllers
{
    public class RazorController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var data = new List<ProductViewModel>();
            data.Add(new ProductViewModel { ID = 1, Name = "Product 1", Code = "P001" });
            data.Add(new ProductViewModel { ID = 2, Name = "Product 2", Code = "P002" });
            data.Add(new ProductViewModel { ID = 3, Name = "Product 3", Code = "P003" });
            data.Add(new ProductViewModel { ID = 4, Name = "Product 4", Code = "P004" });

            return View(data);
        }
    }
}
