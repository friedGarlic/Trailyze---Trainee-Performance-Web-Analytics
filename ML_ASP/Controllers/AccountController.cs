using Microsoft.AspNetCore.Mvc;
using ML_ASP.DataAccess.Repositories.IRepositories;
using ML_ASP.Models;

namespace ML_ASP.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accRepository;

        public AccountController(IAccountRepository accRepository)
        {
            _accRepository = accRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SignIn()
        {

            return View();
        }

        //public async Task SignIn()
        //{
        //    await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,new AuthenticationProperties{RedirectUri = Url.Action("GoogleResponse")});
        //}


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
            var user = _accRepository.GetFirstOrDefault(u => u.Username == username && u.Password == password);

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
				_accRepository.Add(obj);
				_accRepository.Save();
                TempData["success"] = "Account in successfuly created";
                return RedirectToAction("SignIn");
            }
            return View(obj);
        }

        //for google drive that failed
        /*public async Task<IActionResult> GoogleResponse()
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
        }*/
    }
}
