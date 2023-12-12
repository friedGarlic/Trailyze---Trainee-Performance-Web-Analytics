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

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignIn(Account_Model model)
        {
            // Check the username and password against your database or any authentication logic
            if (IsValidUser(model.Username, model.Password))
            {
                // Successful sign-in
                // You might redirect to a dashboard or another page
                return RedirectToAction("Dashboard", "Dashboard");
            }

            // Failed sign-in
            ModelState.AddModelError(string.Empty, "Invalid username or password");
            return View(model);
        }

        private bool IsValidUser(string username, string password)
        {
            var user = _dbContext.Accounts.FirstOrDefault(u => u.Username == username && u.Password == password);

            // If the user is found, consider it a valid user
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
