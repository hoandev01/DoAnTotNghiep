using Microsoft.AspNetCore.Mvc;

namespace ChickenF.Controllers.EmployeeArea
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }

}
