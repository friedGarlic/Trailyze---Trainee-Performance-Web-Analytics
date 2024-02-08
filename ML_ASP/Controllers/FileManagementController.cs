using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Accord.IO;
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
using System;

namespace ML_ASP.Controllers
{
    public class FileManagementController : Controller
    {
        private readonly IUnitOfWork _unit;
        private readonly MLContext _context;
        private readonly PredictionEngine<Object_DataSet, Prediction> _predictionEngine;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _environment;

        public SubmissionVM submissionVM { get; set; }

        public FileManagementController(Microsoft.AspNetCore.Hosting.IWebHostEnvironment environment,
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
        public IActionResult FileManagement()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var getAcc = _unit.Account.GetFirstOrDefault(x => x.Id == claim.Value);
                string? imageUrl = getAcc.ImageUrl;

                ViewData["ImageUrl"] = imageUrl;

                return View();
            }
            else
            {
                return View();
            }
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
        public IActionResult FileManagement(List<IFormFile> postedFiles)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            // --UPLOADING FILE --
            string uploadFolderName = "Uploads";
            string submissionFolderName = "Submission";
            string projectPath = _environment.ContentRootPath;
            string uploadPath = Path.Combine(projectPath, uploadFolderName);
            string submissionPath = uploadPath; // Default to the Uploads folder
            Prediction prediction = null;


            bool submissionIsGreaterThan1 = false;

            if (postedFiles.Count > 1)
            {
                submissionIsGreaterThan1 = true;
                submissionPath = Path.Combine(uploadPath, submissionFolderName);

                if (!Directory.Exists(submissionPath))
                {
                    Directory.CreateDirectory(submissionPath);
                }
                else
                {
                    // If "Submission" folder already exists, find the next available name
                    int attempt = 1;
                    while (Directory.Exists(submissionPath))
                    {
                        submissionFolderName = $"Submission_{attempt}";
                        submissionPath = Path.Combine(uploadPath, submissionFolderName);
                        attempt++;
                    }

                    Directory.CreateDirectory(submissionPath);
                }
            }

            List<string> uploadedFiles = new List<string>();
            foreach (IFormFile postedFile in postedFiles)
            {
                var submissionModel = new SubmissionModel();
                string fileName = Path.GetFileName(postedFile.FileName);
                string filePath = Path.Combine(submissionPath, fileName);

                int attempt = 1;
                while (System.IO.File.Exists(filePath))
                {
                    // Handle file name conflict as needed
                    string extension = Path.GetExtension(fileName);
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                    fileName = $"{fileNameWithoutExtension}_{attempt}{extension}";
                    filePath = Path.Combine(submissionPath, fileName);
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
                string predGrade = prediction.Prediciton.ToString();

                //adding identity for the one who upload the file
                submissionModel.SubmissionUserId = claim.Value;

                //adding folderid if 1 or more files is updated
                if (postedFiles.Count > 1)
                {
                    var fileModel = SubmissionInjection(submissionModel, fileName, predGrade, submissionFolderName, submissionIsGreaterThan1);

                    _unit.Submission.Add(fileModel);
                    _unit.Save();
                }
                else
                {
                    var fileModel = SubmissionInjection(submissionModel, fileName, predGrade, submissionIsGreaterThan1);

                    _unit.Submission.Add(fileModel);
                    _unit.Save();
                }
            }
            //UPLOADING FILES ENDS-------------

            if (postedFiles.Count <= 0)
            {
                TempData["failed"] = "No File was Uploaded";

                return RedirectToAction(nameof(FileManagement));
            }
            //VERIFICATION OF NO UPLOAD SUBMIT ENDS--------------

            submissionVM = new SubmissionVM()
            {
                SubmissionList = _unit.Submission.GetAll(u => u.SubmissionUserId == claim.Value),
                IsMultipleFile = true
            };
            //DATABASE COLLERATION ENDS------------

            TempData["success"] = "Uploaded Succesfully!";

            if (claim != null)
            {
                var getAcc = _unit.Account.GetFirstOrDefault(x => x.Id == claim.Value);
                string? imageUrl = getAcc.ImageUrl;

                ViewData["ImageUrl"] = imageUrl;
            }

            return View(submissionVM);
        }

        public SubmissionModel SubmissionInjection(SubmissionModel submissionModel, string filename, string grade, string submissionFolderName, bool submissionIsGreaterThan1)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var account = _unit.Account.GetFirstOrDefault(u => u.Id == claim.Value);

            submissionModel.Date = DateTime.Now;

            submissionModel.Name = account.FullName;

            submissionModel.FileName = filename;
            submissionModel.ApprovalStatus = "Pending";
            submissionModel.Grade = grade;
            submissionModel.FolderId = submissionFolderName;
            submissionModel.IsMultipleFile = submissionIsGreaterThan1;

            return submissionModel;
        }
        public SubmissionModel SubmissionInjection(SubmissionModel submissionModel, string filename, string grade, bool submissionIsGreaterThan1)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var account = _unit.Account.GetFirstOrDefault(u => u.Id == claim.Value);

            submissionModel.Date = DateTime.Now;

            submissionModel.Name = account.FullName;

            submissionModel.FileName = filename;
            submissionModel.ApprovalStatus = "Pending";
            submissionModel.Grade = grade;
            submissionModel.IsMultipleFile = submissionIsGreaterThan1;

            return submissionModel;
        }

        public ActionResult ViewImage(string fileName)
        {
            if (fileName != null)
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
            else
            {
                return RedirectToAction("Dashboard", "Dashboard");
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

                        return RedirectToAction("Dashboard", "Dashboard");
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

            _unit.Log.Update(logModel, fileName, accountName, id);
            _unit.Save();

            return RedirectToAction("Dashboard", "Dashboard");
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

        //------------------------------ENDPOINT REGIONS ------------------------------//
        #region API CALLS
        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var submissionList = _unit.Submission.GetAll(u => u.SubmissionUserId == claim.Value);
            return Json(new { data = submissionList });
        }

        #endregion
    }
}
