using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ML_ASP.DataAccess.Repositories.IRepositories;
using ML_ASP.Models;
using ML_ASP.Models.Models;
using ML_ASP.Models.ViewModel;
using ML_ASP.Utility;
using System.Security.Claims;

namespace ML_ASP.Controllers
{
	
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unit;
		private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _environment;

		public SubmissionVM submissionVM { get; set; }

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
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			IEnumerable<IdentityUser> accountList = _unit.Account.GetAll();
			IEnumerable<SubmissionModel> modelList = _unit.Submission.GetAll();

			var submissionCount = modelList.Count();
			int accountCount = accountList.Select(u => u.Id).Distinct().Count();

			ViewBag.AccountCount = accountCount;
			ViewBag.SubmissionCount = submissionCount;

			submissionVM = new SubmissionVM()
			{
				ReminderList = _unit.Reminder.GetAll(u => u.UserId == claim.Value)
			};

			return View(submissionVM);
		}

		[Authorize(Roles = SD.Role_Admin)]
		[HttpPost]
		public IActionResult UpdateApprovalStatusBulk(List<int> id, List<string> approvalStatus, List<string> originalApprovalStatus)
		{
			for (int i = 0; i < id.Count; i++)
			{
				int changedId = id[i];
				string newApprovalStatus = approvalStatus[i];

				_unit.Submission.ChangeApprovalStatus(changedId, newApprovalStatus);
			}
			try
			{
				_unit.Save();
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				Console.WriteLine("Exception Message: " + message);
			}

			return RedirectToAction(nameof(Admin));
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
			submissionVM = new SubmissionVM()
			{
				AccountList = getAccounts
			};
			return View(submissionVM);
		}

		[Authorize]
		[HttpPost]
		public ActionResult EditProfile(Guid id, int numberOfHours, int weeklyReport)
		{
			_unit.Account.UpdateAccount(numberOfHours,weeklyReport, id.ToString());
			_unit.Save();

			return RedirectToAction(nameof(Admin));
		}

		[Authorize]
		[HttpPost]
		public IActionResult AddTodoList(string nameOfReminder, string iconType, string iconClass)
		{
			//find the unique current user
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			Reminder_Model reminder_Model = new Reminder_Model();

			//for admin dashboard todolist, populate so it doesnt sve in db as null
			if (iconType == null)
			{
				iconType = "0";
			}

			reminder_Model.Name = nameOfReminder;
			reminder_Model.UserId = claim.Value;
			reminder_Model.IconClass = iconClass;
			reminder_Model.IconType = iconType;

			_unit.Reminder.Add(reminder_Model);
			_unit.Save();

			TempData["success"] = "Added Reminder Succesfully!";

			submissionVM = new SubmissionVM()
			{
				ReminderList = _unit.Reminder.GetAll(u => u.UserId == claim.Value)
			};

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
