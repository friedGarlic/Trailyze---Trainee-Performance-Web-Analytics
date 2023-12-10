using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using ML_net.ModelSession_1;

namespace ML_ASP.Controllers
{
    public class DashboardController : Controller
    {
        private readonly MLContext _context;
        private readonly PredictionEngine<Performance_DataSet, Performance_Prediction> _predictionEngine;

        public DashboardController()
        {
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

        [HttpPost]
        public IActionResult FileManagement(Performance_DataSet input)
        {
            // Use the loaded model to make predictions
            var prediction = _predictionEngine.Predict(input);

            // You can do something with the prediction, e.g., pass it to the view
            ViewBag.Prediction = prediction.PerformancePrediciton_Score;

            return View(input);
        }
    }
}
