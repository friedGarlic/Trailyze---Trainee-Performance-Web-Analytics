using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using ML_ASP.Models;
using System.Diagnostics;

namespace ML_ASP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailSender _emailSender;

        public HomeController(ILogger<HomeController> logger,
            IEmailSender emailSender)
        {
            _emailSender = emailSender;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public ActionResult SendEmail(string fullName, string email, string message)
        {
            var emailTo = "remcarlmerza@gmail.com";
            _emailSender.SendEmailAsync( emailTo, "Information", "FullName: " + fullName + " Email: " + email + "\n" + message);

            return RedirectToAction(nameof(Index));
        }
    }
}