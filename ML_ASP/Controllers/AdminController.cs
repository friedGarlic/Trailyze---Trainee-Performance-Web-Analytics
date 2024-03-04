using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ML_ASP.DataAccess.Repositories.IRepositories;
using ML_ASP.Models;
using ML_ASP.Models.Models;
using ML_ASP.Models.ViewModel;
using ML_ASP.Utility;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Http;
using System.Security.Claims;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using OpenAI_API;
using OpenAI_API.Completions;
using System.CodeDom;
using static Tensorflow.CollectionDef.Types;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ML_ASP.Controllers
{

	public class AdminController : Controller
	{
		private readonly IUnitOfWork _unit;
		private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _environment;
		private readonly IHttpClientFactory _httpClientFactory;

		public SubmissionVM submissionVM { get; set; }

		public AdminController(IUnitOfWork unit,
			Microsoft.AspNetCore.Hosting.IWebHostEnvironment environment,
			IHttpClientFactory httpClientFactory)
		{
			_environment = environment;
			_unit = unit;
			_httpClientFactory = httpClientFactory;
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

			//find last 5 grade from current user
			var sublist = _unit.Submission
				  .GetAll(u => u.SubmissionUserId == claim.Value)
				  .OrderByDescending(u => u.Id) // Assuming there's an ID field for ordering
				  .Take(5)
				  .Select(u => u.Grade)
				  .ToList();

			submissionVM = new SubmissionVM()
			{
				ReminderList = _unit.Reminder.GetAll(u => u.UserId == claim.Value),
				GradeList = sublist
			};

			return View(submissionVM);
		}

		//---------------------ANALYTIC FEATURES METHODS AND GETTERS------------------------------------
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

		public ActionResult ProfileFilter(string searchName)
		{
			var getAccounts = _unit.Account.GetAll();
			
			submissionVM = new SubmissionVM()
			{
				AccountList = getAccounts,
				SearchQuery = searchName
			};

			return View(nameof(Analytics),submissionVM);
		}

		// --------------------METHODS ------------------
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

		[HttpPost]
		public ViewResult GenerateQRCode()  ////////////// currently not used whatsoever
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			QRModel model = new QRModel();
			if (model.QrCode == null)
			{
				var id = _unit.QR.GetFirstOrDefault().Id;
				var killFile = _unit.QR.GetFirstOrDefault(u => u.Id == id);
				_unit.QR.Remove(killFile);
			}

			Random random = new Random();
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			int length = 8;
			var randomString = new string(Enumerable.Repeat(chars, length)
				.Select(s => s[random.Next(s.Length)]).ToArray());

			model.QrCode = randomString;

			//simply do 64bit image ang 64bit string using qrcoder and bitmap
			using (MemoryStream ms = new MemoryStream())
			{
				QRCodeGenerator qrGenerator = new QRCodeGenerator();
				QRCodeData qrData = qrGenerator.CreateQrCode(model.QrCode, QRCodeGenerator.ECCLevel.Q);
				QRCode qrCode = new QRCode(qrData);
				using (Bitmap bitmap = qrCode.GetGraphic(20))
				{
					bitmap.Save(ms, ImageFormat.Png);
					model.QrCode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
					ViewBag.QRCodeImage = model.QrCode;
				}
			}

			_unit.QR.Add(model);
			_unit.Save();

			submissionVM = new SubmissionVM()
			{
				ReminderList = _unit.Reminder.GetAll(u => u.UserId == claim.Value)
			};
			return View(nameof(Admin), submissionVM);
		}

		[Authorize]
		[HttpPost]
		public ActionResult EditProfile(Guid id, int numberOfHours, int weeklyReport, string _course)
		{
			_unit.Account.UpdateAccount(_course, numberOfHours, weeklyReport, id.ToString());
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

		[HttpGet]
		public async Task<IActionResult> SendToApi()
		{
			string OutputResult = "";
			try
			{
				string apiKey = "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";
				var openai = new OpenAIAPI(apiKey);
				var request = openai.Chat.CreateConversation();
				request.AppendUserInput("The grade of student is 80/100,77/100,80/100. What is your analysis on this.");
				var response = await request.GetResponseFromChatbotAsync();
				
				foreach (var message in response)
				{
					OutputResult += message.ToString();
				}

				return Json(new { data = OutputResult });
			}
			catch (Exception ex)
			{
				// Log any exceptions
				Console.WriteLine(ex.Message);
				return StatusCode(500, ex.Message); // Internal Server Error
			}
		}

		#endregion


	}
}
