using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ML_ASP.Data;
using ML_ASP.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.Google;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Http;
using Accord;
using iText.IO.Source;

namespace ML_ASP.Controllers
{
    public class AccountController : Controller
    {
        private readonly ML_DBContext _dbContext;

        public AccountController(ML_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task SignIn()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleResponse")
                });
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var claim = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });

            return RedirectToAction("FileManagement", "Dashboard", new { area = "" });
        }

        [HttpPost]
        public IActionResult SignIn(Account_Model model)
        {
            if (IsValidUser(model.Username, model.Password))
            {
                return RedirectToAction("Dashboard", "Dashboard");
            }

            ModelState.AddModelError(string.Empty, "Invalid username or password");
            return View(model);
        }

        private bool IsValidUser(string username, string password)
        {
            var user = _dbContext.Accounts.FirstOrDefault(u => u.Username == username && u.Password == password);

            return user != null;
        }

        public IActionResult SignUp()
        {
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignUp(Account_Model obj)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Accounts.Add(obj);
                _dbContext.SaveChanges();
                TempData["success"] = "Account in successfuly created";
                return RedirectToAction("SignIn");
            }
            return View(obj);
        }
    }
}
