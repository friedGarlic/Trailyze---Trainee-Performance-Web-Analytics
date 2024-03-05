﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using ML_ASP.DataAccess.Repositories.IRepositories;
using ML_ASP.Models.Models;
using ML_ASP.Models.ViewModel;
using System.Security.Claims;
//using ML_net.ModelSession_1;
using ML_net.ModelSession_2;

//
using QRCoder;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using iText.Barcodes.Qrcode;
using ML_net.ModelSession_3;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using System.ComponentModel.DataAnnotations;

namespace ML_ASP.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unit;
        private readonly MLContext _context;
        private readonly PredictionEngine<Object_DataSet, Prediction> _predictionEngine;
        private readonly PredictionEngine<Image_DataSet, ImagePrediction> _imagClassificationEngine;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _environment;

        public SubmissionVM submissionVM { get; set; }

        public DashboardController(Microsoft.AspNetCore.Hosting.IWebHostEnvironment environment, 
            IUnitOfWork unit)
        {
            _unit = unit;
            _environment = environment;
            _context = new MLContext(); //was supposed to be DB, but the architecture was applied late

            //find current file folder path
            var currentDirectory = _environment.ContentRootPath;

            string desiredDirectory = "ClassLibrary1";
            string modelDirectory = "ModelSession_2";
            while (!Directory.Exists(Path.Combine(currentDirectory, desiredDirectory)))
            {
                currentDirectory = Directory.GetParent(currentDirectory).FullName;
            }

            currentDirectory = Path.Combine(currentDirectory, desiredDirectory);
            // Construct the path relative to the desired directory
            string combinePath = Path.Combine(currentDirectory, modelDirectory);
            string modelPath = Path.Combine(combinePath, "GradePrediction.zip");

            //for deployment mode only or when published -------------------=====================
            //var modelPath = "C:\\inetpub\\wwwroot\\trailyze\\ModelSession_1\\GradePrediction.zip";
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

            //for grades
            var sublist = _unit.Submission
                  .GetAll(u => u.SubmissionUserId == claim.Value)
                  .Take(5)
                  .Select(u => u.Grade)
                  .ToList();
            //-------------------grade ends


            //reminder alarm
            var getaccounttime = _unit.Reminder.GetAll(u => u.UserId == userId);

            foreach (var i in getaccounttime.ToList())
            {
                DateTime currentTime = DateTime.Now;
                if (i.ReminderDateTime > currentTime)
                {
                    TimeSpan duration = (TimeSpan)(i.ReminderDateTime - currentTime);
                    double durationInSeconds = duration.TotalMilliseconds;

                    _unit.Reminder.Update(i.Id, durationInSeconds);
                }
            }
            //-------------------reminder alarm ends

            submissionVM = new SubmissionVM()
            {
                LogList = _unit.Log.GetAll(u => u.LogId == claim.Value),
                ReminderList = _unit.Reminder.GetAll(u => u.UserId == claim.Value),
                GradeList = sublist,
                CurrentUserId = userId,
            };
            //Get Account List and name ends --------------------------

            //retrieve last entry
            //so it stays as Timed in if timed in-------------------
            var lastLogEntry = _unit.Log.GetAll(u => u.LogId == claim.Value)
                                   .OrderByDescending(u => u.DateTime)
                                   .FirstOrDefault();

            bool initialIsTimedIn = lastLogEntry != null && lastLogEntry.Log == "Timed In";

            ViewBag.InitialIsTimedIn = initialIsTimedIn;
            //-------------------log ends

            //getters
            var submission = _unit.Submission.GetAll(u => u.SubmissionUserId == userId);
            int submissionCount = submission.Count();

            ViewBag.SubmissionCount = submissionCount;
            ViewBag.RemainingReports = account.WeeklyReportRemaining;

            //remaining
            ViewBag.RemainingHours = account.HoursRemaining;
            ViewBag.RemainingMinutes = account.MinutesRemaining;
            ViewBag.RemainingSeconds = account.SecondsRemaining;
            //completed
            ViewBag.HoursCompleted = account.HoursCompleted;
            ViewBag.MinutesCompleted = account.MinutesCompleted;
            ViewBag.SecondsCompleted = account.SecondsCompleted;

            //check if theres log in account
            //post picture if there is
            if (claim != null)
            {
                var getAcc = _unit.Account.GetFirstOrDefault(x => x.Id == claim.Value);
                string? imageUrl = getAcc.ImageUrl;

                ViewData["ImageUrl"] = imageUrl;
            }

            _unit.Save();

            return View(submissionVM);
        }


        // --------- ONLY FOR TIME LOG PURPOSES -----------------
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

            if (IsTimedIn == true)
            {
                InputTimeDuration();
            }

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

        //-----------------ADD ON FEATURES ---------------------
        [HttpPost]
        public ActionResult AddReminder(string nameOfReminder,string iconType, string iconClass, DateTime dateTime)
        {
            //find the unique current user
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            Reminder_Model reminder_Model = new Reminder_Model();

            DateTime currentTime = DateTime.Now;
            TimeSpan duration = dateTime - currentTime;
            double durationInSeconds = duration.TotalMilliseconds;

            if (durationInSeconds > 0 && !string.IsNullOrEmpty(nameOfReminder) && !string.IsNullOrEmpty(iconClass) &&
                    !string.IsNullOrEmpty(iconType) && !string.IsNullOrEmpty(claim.Value) && dateTime != null)
            {
                reminder_Model.ReminderDuration = durationInSeconds;
                reminder_Model.Name = nameOfReminder;
                reminder_Model.UserId = claim.Value;
                reminder_Model.IconClass = iconClass;
                reminder_Model.IconType = iconType;
                reminder_Model.ReminderDateTime = dateTime;

                _unit.Reminder.Add(reminder_Model);
                _unit.Save();
            }

            TempData["success"] = "Added Reminder Succesfully!";

            return RedirectToAction(nameof(Dashboard));
        }


        //-----------------HELPER FUNCTIONS OR METHODS--------------------------
        [HttpPost]
        public ActionResult DeleteReminder(int id)
        {
            var killFile = _unit.Reminder.GetFirstOrDefault(u => u.Id == id);
            _unit.Reminder.Remove(killFile);
            _unit.Save();

            TempData["success"] = "Delete Reminder Succesfully!";

            return RedirectToAction(nameof(Dashboard));
        }

        //for calculation in time duration between Timein/timout
        public void InputTimeDuration()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var account = _unit.Account.GetFirstOrDefault(u => u.Id == claim.Value);

            //populate account hrs if null
            PopulateTime();

            var lastTimedInEntry = _unit.Log.GetAll(u => u.LogId == claim.Value && u.Log == "Timed In")
                                    .OrderByDescending(u => u.DateTime)
                                    .FirstOrDefault();

            var lastTimedOutEntry = _unit.Log.GetAll(u => u.LogId == claim.Value && u.Log == "Timed Out")
                                     .OrderByDescending(u => u.DateTime)
                                     .FirstOrDefault();

            
            TimeSpan fullDuration = lastTimedOutEntry.DateTime - lastTimedInEntry.DateTime;

            int totalDurationSeconds = (int)fullDuration.TotalSeconds;
            int hours = totalDurationSeconds / 3600; // Calculate hours from total seconds
            totalDurationSeconds %= 3600;            // Remaining seconds after subtracting hours
            int minutes = totalDurationSeconds / 60; // Calculate minutes from remaining seconds
            int seconds = totalDurationSeconds % 60;

            //completed
            var totalHours = account.HoursCompleted + hours;
            var totalMinutes = account.MinutesCompleted + minutes;
            var totalSeconds = account.SecondsCompleted + seconds;

            //remianing
            var totalRemainingH = account.HoursRemaining - hours;
            var totalRemainingM = account.MinutesRemaining - minutes;
            var totalRemainingS = account.SecondsRemaining - seconds;

            if (totalRemainingS < 0)
            {
                totalRemainingM -= 1;
                totalRemainingS += 60;
            }

            if (totalRemainingM < 0)
            {
                totalRemainingH -= 1;
                totalRemainingM += 60;
            }

            _unit.Account.UpdateTime(totalHours, totalMinutes, totalSeconds,
                totalRemainingH, totalRemainingM, totalRemainingS,
                fullDuration, account.Id);
            _unit.Save();
        }

        //check if h/m/s duration is null
        public void PopulateTime()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var account = _unit.Account.GetFirstOrDefault(u => u.Id == claim.Value);

            //completed
            if(account.HoursCompleted == null)
            {
                account.HoursCompleted = 0;
            }
            if(account.MinutesCompleted == null)
            {
                account.MinutesCompleted = 0;
            }
            if(account.SecondsCompleted == null)
            {
                account.SecondsCompleted = 0;
            }

            //remaining
            if(account.HoursRemaining == null)
            {
                account.HoursRemaining = 0;
            }
            if(account.MinutesRemaining == null)
            {
                account.MinutesRemaining = 0;
            }
            if (account.SecondsRemaining == null)
            {
                account.SecondsRemaining = 0;
            }
        }
    }
}

