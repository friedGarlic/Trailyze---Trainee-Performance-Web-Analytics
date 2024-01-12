using Accord.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

            var submission = _unit.Submission.GetAll(u => u.SubmissionUserId == userId);
            int submissionCount = submission.Count();

            ViewBag.SubmissionCount = submissionCount;
            //
            ViewBag.RemainingHours = account.HoursRemaining;
            ViewBag.RemainingReports = account.WeeklyReportRemaining;

            //for development purposes
            //if(account.WeeklyReportRemaining == null)
            //{
            //    account.WeeklyReportRemaining = 20;
            //    _unit.Account.Add(account);
            //    _unit.Save();
            //}

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

            return View(submissionVM);
        }

        [Authorize]
        public IActionResult FileManagement()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            submissionVM = new SubmissionVM()
            {
                SubmissionList = _unit.Submission.GetAll(u=> u.SubmissionUserId == claim.Value)
            };

            //TODO show all submission of an guid that is equal to the guid of login
            return View(submissionVM);
        }

        [Authorize]
        [HttpPost]
        public ActionResult DeleteFile(int id, string fileName)
        {
            var killFile = _unit.Submission.GetFirstOrDefault(u => u.Id == id);
            _unit.Submission.Remove(killFile);
            _unit.Save();
            string path = Path.Combine(_environment.ContentRootPath + "\\Uploads", fileName);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            TempData["failed"] = "File Deleted";

            return RedirectToAction(nameof(FileManagement));
        }

        [HttpPost]
        [Authorize]
        public IActionResult FileManagement(List<IFormFile> postedFiles, SubmissionModel submissionModel)
        {
            // --UPLOADING FILE --
            string uploadFolderName = "Uploads";
            string projectPath = _environment.ContentRootPath;
            string path = Path.Combine(projectPath, uploadFolderName);
            string fileName = "";
            Prediction prediction = null;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            List<string> uploadedFiles = new List<string>();
            foreach (IFormFile postedFile in postedFiles)
            {
                fileName = Path.GetFileName(postedFile.FileName);
                string filePath = Path.Combine(path, fileName);

                int attempt = 1;
                while (System.IO.File.Exists(filePath))
                {
                    // if file exists, prompt user for a new name
                    string extension = Path.GetExtension(fileName);
                    if (extension != ".pdf")
                    {
                        TempData["failed"] = "Can Only Upload PDF FILES!!";

                        return RedirectToAction(nameof(FileManagement));
                    }
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                    fileName = $"{fileNameWithoutExtension}_{attempt}{extension}";
                    filePath = Path.Combine(path, fileName);
                    attempt++;
                }

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                    uploadedFiles.Add(fileName);
                    ViewBag.Message += string.Format("<b>{0}</b> uploaded.<br />", fileName);
                }

                int numWordsInPdf = Demo.CountSpacesInPdf(filePath);

                Random rnd = new Random();
                int num = rnd.Next(80, 90);

                var new_data = new Object_DataSet
                {
                    NumberOfWords = numWordsInPdf,
                    Grade = num
                };

                prediction = _predictionEngine.Predict(new_data);

            }
            //UPLOADING FILES ENDS-------------

            if (postedFiles.Count <= 0)
            {
                TempData["failed"] = "No File was Uploaded";

                return RedirectToAction(nameof(FileManagement));
            }
            //VERIFICATION OF NO UPLOAD SUBMIT ENDS--------------


            //adding identity for the one who upload the file
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            submissionModel.SubmissionUserId = claim.Value;

            var fileModel = SubmissionInjection(submissionModel, fileName);

            _unit.Submission.Add(fileModel);
            _unit.Save();
            //DATABASE COLLERATION ENDS------------

            TempData["success"] = "Uploaded Succesfully!";

            submissionVM = new SubmissionVM()
            {
                SubmissionList = _unit.Submission.GetAll(u => u.SubmissionUserId == claim.Value),
                Prediction = prediction.Prediciton.ToString()
            };

            return View(submissionVM);
        }

        public SubmissionModel SubmissionInjection(SubmissionModel submissionModel, string filename)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var account = _unit.Account.GetFirstOrDefault(u => u.Id == claim.Value);

            submissionModel.Date = DateTime.Now;

            submissionModel.Name = account.FullName;

            submissionModel.FileName = filename;
            submissionModel.ApprovalStatus = "Pending";

            return submissionModel;
        }

        public ActionResult ViewImage(string fileName)
        {
            string path = Path.Combine(_environment.ContentRootPath + "\\Images", fileName);
            string contentType = GetContentType(fileName);

            if (System.IO.File.Exists(path))
            {
                return File(System.IO.File.ReadAllBytes(path), contentType);
            }
            else
            {
                TempData["failed"] = "File Not Found";
                return NotFound();
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult UploadImage(List<IFormFile> postedImages, int id)
        {
            string uploadFolderName = "Images";
            string projectPath = _environment.ContentRootPath;
            string path = Path.Combine(projectPath, uploadFolderName);
            string fileName = "";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            List<string> uploadedImages = new List<string>();
            foreach (IFormFile postedImage in postedImages)
            {
                fileName = Path.GetFileName(postedImage.FileName);
                string filePath = Path.Combine(path, fileName);

                int attempt = 1;
                while (System.IO.File.Exists(filePath))
                {
                    // If the file exists, prompt the user for a new name
                    string extension = Path.GetExtension(fileName);
                    if (!IsImageFile(extension))
                    {
                        TempData["failed"] = "Can Only Upload Image FILES!!";

                        return RedirectToAction(nameof(Dashboard));
                    }

                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                    fileName = $"{fileNameWithoutExtension}_{attempt}{extension}";
                    filePath = Path.Combine(path, fileName);
                    attempt++;
                }

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    postedImage.CopyTo(stream);
                    uploadedImages.Add(fileName);
                    ViewBag.Message += string.Format("<b>{0}</b> uploaded.<br />", fileName);
                }
            }

            LogModel logModel = new LogModel();

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier); 
            var account = _unit.Account.GetFirstOrDefault(u => u.Id == claim.Value);
            var accountName = account.FullName;

            logModel.ImageUrl = claim.Value;

            _unit.Log.Update(logModel, fileName,accountName, id);
            _unit.Save();

            return RedirectToAction(nameof(Dashboard));
        }
        // ------------------------ HELPER METHODS ------------------------------------//
        private bool IsImageFile(string extension)
        {
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            return imageExtensions.Contains(extension.ToLower());
        }

        private string GetContentType(string fileName)
        {
            string extension = Path.GetExtension(fileName)?.ToLowerInvariant();

            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".gif":
                    return "image/gif";
                // add more cases for other image formats as needed
                default:
                    return "application/octet-stream";
            }
        }
    }
}

