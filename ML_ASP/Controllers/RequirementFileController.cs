using Microsoft.AspNetCore.Mvc;
using ML_ASP.DataAccess.Repositories.IRepositories;

namespace ML_ASP.Controllers
{
    public class RequirementFileController : Controller
    {
        public readonly IUnitOfWork _unit;
        public RequirementFileController(IUnitOfWork unit)
        {
            _unit = unit;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PermissionDenied()
        {
            return View();
        }
    }
}
