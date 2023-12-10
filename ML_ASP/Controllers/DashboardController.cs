using Microsoft.AspNetCore.Mvc;

namespace ML_ASP.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult FileManagement()
        {
            return View();
        }

        //TODO try fix when auto grading is integrated to not crash
    }
}
