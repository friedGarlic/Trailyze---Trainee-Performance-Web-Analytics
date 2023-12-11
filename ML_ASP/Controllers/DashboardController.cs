using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using ML_net.ModelSession_1;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ML_ASP.Controllers
{
    public class DashboardController : Controller
    {
        private readonly MLContext _context;
        private readonly PredictionEngine<Performance_DataSet, Performance_Prediction> _predictionEngine;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment;

        public DashboardController(Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment)
        {
            Environment = _environment;
            _context = new MLContext();

            // load the trained model
            var modelPath = "C:\\Users\\rem\\source\\repos\\OJTPERFORMANCE-ASP-ML.NET-master\\ClassLibrary1\\bin\\Debug\\net7.0\\ModelSession_1\\PerformancePrediction.zip";
            var trainedModel = _context.Model.Load(modelPath, out var modelSchema);

            // prediction engine
            _predictionEngine = _context.Model.CreatePredictionEngine<Performance_DataSet, Performance_Prediction>(trainedModel);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult FileManagement()
        {
            return View();
        }

        /*[HttpPost]
        public IActionResult FileManagement(Performance_DataSet input)
        {
            // Use the loaded model to make predictions
            var prediction = _predictionEngine.Predict(input);

            // You can do something with the prediction, e.g., pass it to the view
            ViewBag.Prediction = prediction.PerformancePrediciton_Score;

            return View(input);
        }*/

        [HttpPost]
        public IActionResult FileManagement(List<IFormFile> postedFiles)
        {
            // The folder name where you want to store the uploads within your project
            string uploadFolderName = "Uploads";

            // Get the absolute path to the project directory
            string projectPath = Path.Combine(this.Environment.ContentRootPath, "ML_ASP");

            // Combine the project path with the folder where you want to store uploads
            string path = Path.Combine(projectPath, uploadFolderName);

            // Check if the folder exists, create it if not
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            List<string> uploadedFiles = new List<string>();

            foreach (IFormFile postedFile in postedFiles)
            {
                string fileName = Path.GetFileName(postedFile.FileName);

                // Combine the upload folder path with the file name
                string filePath = Path.Combine(path, fileName);

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                    uploadedFiles.Add(fileName);
                    ViewBag.Message += string.Format("<b>{0}</b> uploaded.<br />", fileName);
                }
            }

            return View();
        }

    }
}
