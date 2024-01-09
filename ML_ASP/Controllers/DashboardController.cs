﻿using Accord.IO;
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

            submissionVM = new SubmissionVM()
            {
                SubmissionList = _unit.Submission.GetAll(u => u.SubmissionUserId == claim.Value)
            };

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
                    if(extension != ".pdf")
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

                var prediction = _predictionEngine.Predict(new_data);

                ViewBag.Prediction = prediction.Prediciton;
            }

            if (postedFiles.Count <= 0)
            {
                TempData["failed"] = "No File was Uploaded";

                return RedirectToAction(nameof(FileManagement));
            }

            //adding identity for the one who upload the file
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            submissionModel.SubmissionUserId = claim.Value;


            var fileModel = SubmissionInjection(submissionModel, fileName);
            _unit.Submission.Add(fileModel);

            _unit.Save();
            TempData["success"] = "Uploaded Succesfully!";

            return RedirectToAction(nameof(FileManagement)); // Make sure to return a view or redirect after the upload
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

        [HttpPost]
        public IActionResult TimeIn()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var account = _unit.Account.GetFirstOrDefault(u => u.Id == claim.Value);

            LogModel logModel = new LogModel
            {
                LogId = account.Id,
                DateTime = DateTime.Now,
                Log = "Log In",
                FullName = account.FullName  // Assuming FullName is a property in AccountModel
            };

            TempData["success"] = "Logged In Succesfully!";

            //_unit.Log.Add(logModel);
            //_unit.Save();

            //TODO this function is not adding to db for development purposes
            return RedirectToAction(nameof(Dashboard)); 
        }
    }
}

