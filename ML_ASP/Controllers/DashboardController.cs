using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using ML_ASP.DataAccess.Repositories.IRepositories;
using ML_ASP.Models;
//using ML_net.ModelSession_1;
using ML_net.ModelSession_2;

namespace ML_ASP.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IAccountRepository _accRepository;
        private readonly MLContext _context;
        private readonly PredictionEngine<Object_DataSet, Prediction> _predictionEngine;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _environment;

        public DashboardController(Microsoft.AspNetCore.Hosting.IWebHostEnvironment environment, ISubmissionRepository submissionRepository
            , IAccountRepository accRepository)
        {
            _accRepository = accRepository;
            _submissionRepository = submissionRepository;
            _environment = environment;
            _context = new MLContext(); //was supposed to be DB, but the architecture was applied late

            var modelPath = "C:\\Users\\rem\\source\\repos\\OJTPERFORMANCE-ASP-ML.NET-master\\ClassLibrary1\\ModelSession_2\\GradePrediction.zip";
            var trainedModel = _context.Model.Load(modelPath, out var modelSchema);

            _predictionEngine = _context.Model.CreatePredictionEngine<Object_DataSet, Prediction>(trainedModel);
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult FileManagement()
        {
            IEnumerable<SubmissionModel> modelList = _submissionRepository.GetAll();
            return View(modelList);
        }

        public ActionResult DeleteFile(string fileName)
        {
            string path = Path.Combine(_environment.ContentRootPath + "\\Uploads", fileName);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            return RedirectToAction("FileManagement");
        }

        [HttpPost]
        public IActionResult FileManagement(List<IFormFile> postedFiles, SubmissionModel submissionModel,Account_Model accModel)
        {
            string uploadFolderName = "Uploads";
            string projectPath = _environment.ContentRootPath;
            string path = Path.Combine(projectPath, uploadFolderName);

            if (!Directory.Exists(path))
            { 
                Directory.CreateDirectory(path);
            }

            List<string> uploadedFiles = new List<string>();
            string? fileName = null;
            foreach (IFormFile postedFile in postedFiles)
            {
                fileName = Path.GetFileName(postedFile.FileName);
                string filePath = Path.Combine(path, fileName);

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

            var fileModel = SubmissionInjection(submissionModel,fileName,_accRepository.GetFirstAndDefault());
            _submissionRepository.Add(fileModel);
            //TODO save ONLY IF admin approve it
            _submissionRepository.Save();


            IEnumerable<SubmissionModel> modelList = _submissionRepository.GetAll();
            return View(modelList);
        }

        public SubmissionModel SubmissionInjection(SubmissionModel submissionModel, string filename, Account_Model accModel)
        {
            submissionModel.Date = DateTime.Now;

            submissionModel.Name = submissionModel.Name ?? accModel.FullName;

            submissionModel.FileName = filename;
            submissionModel.ApprovalStatus = "Pending";

            return submissionModel;
        }
    }
}
