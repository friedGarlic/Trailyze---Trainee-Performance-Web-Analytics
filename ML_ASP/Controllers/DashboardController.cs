using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
//using ML_net.ModelSession_1;
using ML_net.ModelSession_2;

namespace ML_ASP.Controllers
{
    public class DashboardController : Controller
    {
        private readonly MLContext _context;
        private readonly PredictionEngine<Object_DataSet, Prediction> _predictionEngine;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _environment;

        public DashboardController(Microsoft.AspNetCore.Hosting.IWebHostEnvironment environment)
        {
            _environment = environment;
            _context = new MLContext();

            // load the trained model
            var modelPath = "C:\\Users\\rem\\source\\repos\\OJTPERFORMANCE-ASP-ML.NET-master\\ClassLibrary1\\ModelSession_2\\GradePrediction.zip";
            var trainedModel = _context.Model.Load(modelPath, out var modelSchema);

            // prediction engine
            _predictionEngine = _context.Model.CreatePredictionEngine<Object_DataSet, Prediction>(trainedModel);
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
            string uploadFolderName = "Uploads";
            string projectPath = _environment.ContentRootPath;
            string path = Path.Combine(projectPath, uploadFolderName);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            List<string> uploadedFiles = new List<string>();

            foreach (IFormFile postedFile in postedFiles)
            {
                string fileName = Path.GetFileName(postedFile.FileName);
                string filePath = Path.Combine(path, fileName);

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                    uploadedFiles.Add(fileName);
                    ViewBag.Message += string.Format("<b>{0}</b> uploaded.<br />", fileName);
                }

                // Read the number of spaces in the PDF
                int numWordsInPdf = Demo.CountSpacesInPdf(filePath);

                // Rest of your code...
                Random rnd = new Random();
                int num = rnd.Next(10, 50);

                var new_data = new Object_DataSet
                {
                    NumberOfWords = numWordsInPdf,
                    Grade = num
                };

                var prediction = _predictionEngine.Predict(new_data);

                ViewBag.Prediction = prediction.Prediciton;
            }

            return View();
        }


    }
}
