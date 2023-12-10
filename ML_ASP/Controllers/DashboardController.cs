using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using ML_net;

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
            var modelPath = "path/to/your/saved/model";
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

        //TODO try fix when auto grading is integrated to not crash
    }
}
