using iText.Layout.Element;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ML_ASP.DataAccess.Repositories.IRepositories;
using ML_ASP.etc;
using ML_ASP.Models;
using System.CodeDom;
using System.Drawing;
using System.Security.Claims;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace ML_ASP.Controllers
{
	
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unit;
		private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _environment;

		public AdminController(IUnitOfWork unit,
			Microsoft.AspNetCore.Hosting.IWebHostEnvironment environment)
        {
			_environment = environment;
            _unit = unit;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Admin()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			IEnumerable<IdentityUser> accountList = _unit.Account.GetAll();
			IEnumerable<SubmissionModel> modelList = _unit.Submission.GetAll();

			List<string> options = new List<string> { "Approved", "Declined", "Remake" };

			IEnumerable<SelectListItem> submissionList = options.Select(option => new SelectListItem
			{
				Text = option,
				Value = option
			});

			var submissionCount = modelList.Count();
			int accountCount = accountList.Select(u => u.Id).Distinct().Count();

			ViewBag.AccountCount = accountCount;
			ViewBag.SubmissionCount = submissionCount;
            ViewBag.submissionList = submissionList;

			return View(modelList);
        }

        [Authorize]
        [HttpPost]
		public IActionResult UpdateApprovalStatusBulk(List<int> id, List<string> approvalStatus, List<string> originalApprovalStatus)
		{
			for (int i = 0; i < id.Count; i++)
			{
				if (originalApprovalStatus[i] != approvalStatus[i])
				{
					int changedId = id[i];
					string newApprovalStatus = approvalStatus[i];

					_unit.Submission.ChangeApprovalStatus(changedId, newApprovalStatus);
				}
			}

			_unit.Save();

			return RedirectToAction("Admin");
		}

        [Authorize]
        public ActionResult ViewPdf(string fileName)
		{
			string path = Path.Combine(_environment.ContentRootPath + "\\Uploads", fileName);

			if (System.IO.File.Exists(path))
			{
				return File(System.IO.File.ReadAllBytes(path), "application/pdf");
			}
			else
			{
				TempData["failed"] = "File Not Found";
				return NotFound();
			}
		}

		[Authorize]
		public IActionResult Analytics()
		{
			var getAccounts = _unit.Account.GetAll();

			return View(getAccounts);
		}

		[Authorize]
		[HttpPost]
		public ActionResult EditProfile(int numberOfHours, string name)
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			_unit.Account.UpdateAccount(numberOfHours, claim.Value);
			_unit.Save();

			return RedirectToAction(nameof(Admin));
		}
    }
}
