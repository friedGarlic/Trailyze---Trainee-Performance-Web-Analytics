using Accord.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.ML;
using ML_ASP.DataAccess.Repositories;
using ML_ASP.DataAccess.Repositories.IRepositories;
using ML_ASP.Models;
using ML_ASP.Models.Models;
using ML_ASP.Models.ViewModel;
//using ML_net.ModelSession_1;
using ML_net.ModelSession_2;
using System.Security.Claims;

namespace ML_ASP.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unit;
        private readonly MLContext _context;
        private readonly PredictionEngine<Object_DataSet, Prediction> _predictionEngine;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _environment;

        public SubmissionVM submissionVM { get; set; }

        public DashboardController(Microsoft.AspNetCore.Hosting.IWebHostEnvironment environment, 
            IUnitOfWork unit)
        {
            _unit = unit;
            _environment = environment;
            _context = new MLContext(); //was supposed to be DB, but the architecture was applied late

            var modelPath = "C:\\Users\\rem\\source\\repos\\OJTPERFORMANCE-ASP-ML.NET-master\\ClassLibrary1\\ModelSession_2\\GradePrediction.zip";
            var trainedModel = _context.Model.Load(modelPath, out var modelSchema);

            _predictionEngine = _context.Model.CreatePredictionEngine<Object_DataSet, Prediction>(trainedModel);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Dashboard()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var account = _unit.Account.GetFirstOrDefault(u => u.Id == claim.Value); //userid

            var userId = claim.Value;
			var accountName = account.FullName;

            
            ViewBag.AccountName = accountName;

            submissionVM = new SubmissionVM()
            {
                LogList = _unit.Log.GetAll(u => u.LogId == claim.Value)
            };
            //Get Account List and name ends --------------------------

            //retrieve last entry
            //so it stays as Timed in if timed in
            var lastLogEntry = _unit.Log.GetAll(u => u.LogId == claim.Value)
                                   .OrderByDescending(u => u.DateTime)
                                   .FirstOrDefault();

            bool initialIsTimedIn = lastLogEntry != null && lastLogEntry.Log == "Timed In";

            ViewBag.InitialIsTimedIn = initialIsTimedIn;

            //getters
            var submission = _unit.Submission.GetAll(u => u.SubmissionUserId == userId);
            int submissionCount = submission.Count();

            ViewBag.SubmissionCount = submissionCount;
            //
            ViewBag.RemainingHours = account.HoursRemaining;
            ViewBag.RemainingReports = account.WeeklyReportRemaining;

            string imageUrl = _unit.Account.GetFirstOrDefault(x => x.Id == claim.Value)?.ImageUrl;

            ViewData["ImageUrl"] = imageUrl;

            return View(submissionVM);
        }


        // --------- ONLY FOR TIME LOG PURPOSES-----------------
        [HttpPost]
        [Authorize]
        public IActionResult Dashboard(bool IsTimedIn)
        {
            //stuck 2nd when trying to time out IsTimedIn stays true
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var account = _unit.Account.GetFirstOrDefault(u => u.Id == claim.Value);

            var accountName = account.FullName;

            ViewBag.AccountName = accountName;

            LogModel logModel = new LogModel();

            if (IsTimedIn == false)
            {
                logModel.LogId = account.Id;
                logModel.DateTime = DateTime.Now;
                logModel.Log = "Timed In";
                logModel.FullName = account.FullName;

                TempData["success"] = "Timed In Succesfully!";
            }
            else
            {
                logModel.LogId = account.Id;
                logModel.DateTime = DateTime.Now;
                logModel.Log = "Timed Out";
                logModel.FullName = account.FullName;

                TempData["success"] = "Timed Out Succesfully!";
            }

            submissionVM = new SubmissionVM()
            {
                TimeLog = logModel.Log,
                LogList = _unit.Log.GetAll(u => u.LogId == claim.Value)
            };

            _unit.Log.Add(logModel);
            _unit.Save();

            //TODO this function is not adding to db for development purposes

            return RedirectToAction(nameof(Dashboard));
        }

        [Authorize]
        [HttpPost]
        public ActionResult UploadImage(IFormFile file)
		{
            //find the unique user
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			var account = _unit.Account.GetFirstOrDefault(u => u.Id == claim.Value);
            //

			if (file != null && file.Length > 0)
            {
                string projectPath = _environment.WebRootPath;
                string uploadFolderName = "ProfileImages";
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(projectPath, uploadFolderName);
                var extension = Path.GetExtension(file.FileName);

                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }
                 
                if(account.ImageUrl != null)
                {
					var oldImagePath = Path.Combine(projectPath, account.ImageUrl.TrimStart('/'));

					if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var fileStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                // Update the image URL in the model
                var imageUrl = Path.Combine("/", uploadFolderName, fileName + extension);
                account.ImageUrl = imageUrl;

                _unit.Account.Update(account, account.Id);

                _unit.Save();
                TempData["success"] = "Uploaded Successfully!";
            }
            else
            {
                TempData["error"] = "No file uploaded.";
            }

            return RedirectToAction(nameof(Dashboard));
        }

    }
}

