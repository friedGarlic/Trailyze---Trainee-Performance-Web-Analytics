using Microsoft.AspNetCore.Mvc;
using ML_ASP.Data;
using ML_ASP.Models;
using System.ComponentModel.DataAnnotations;

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

        public IActionResult SignIn()
        {
            return View();
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
