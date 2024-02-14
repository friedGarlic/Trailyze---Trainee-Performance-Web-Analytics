using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ML_ASP.DataAccess.Repositories.IRepositories;
using ML_ASP.etc;
using ML_ASP.Models;
using ML_ASP.Utility;

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

        [Authorize(Roles = SD.Role_Admin)]
		public IActionResult Admin()
		{
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

			return View();
		}

		[Authorize(Roles = SD.Role_Admin)]
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

		[Authorize(Roles = SD.Role_Admin)]
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

		[Authorize(Roles = SD.Role_Admin)]
		public ActionResult ViewPdf2(string id)
		{
			string path = Path.Combine(_environment.ContentRootPath + "\\Uploads", id);

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


		[Authorize(Roles = SD.Role_Admin)]
		public IActionResult Analytics()
		{
			var getAccounts = _unit.Account.GetAll();

			return View(getAccounts);
		}

		[Authorize]
		[HttpPost]
		public ActionResult EditProfile(Guid id, int numberOfHours, int weeklyReport)
		{
			_unit.Account.UpdateAccount(numberOfHours,weeklyReport, id.ToString());
			_unit.Save();

			return RedirectToAction(nameof(Admin));
		}

		
		//------------------------------ENDPOINT REGIONS ------------------------------//
		#region API CALLS
		[Authorize]
		[HttpGet]
		public IActionResult GetAll()
		{
			var modelList = _unit.Submission.GetAll();
			return Json(new { data = modelList });
		}
		#endregion
	}
}
