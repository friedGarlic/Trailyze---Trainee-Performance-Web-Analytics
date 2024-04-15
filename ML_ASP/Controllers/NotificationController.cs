using Microsoft.AspNetCore.Mvc;

namespace ML_ASP.Controllers
{
    public class NotificationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetAllNotification()
        {
            return Ok();
        }
    }
}
