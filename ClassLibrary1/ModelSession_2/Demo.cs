using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf;
using Accord.MachineLearning;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
using Microsoft.ML;
using ML_net.ModelSession_1;

namespace ML_net.ModelSession_2
{
    public class Demo
    {
        public static int CountSpacesInPdf(string filePath)
        {
            int totalNumberOfSpaces = 0;

            using (PdfReader pdfReader = new PdfReader(filePath))
            {
                using (PdfDocument pdfDocument = new PdfDocument(pdfReader))
                {
                    for (int page_number = 1; page_number <= pdfDocument.GetNumberOfPages(); page_number++)
                    {
                        var strategy = new SimpleTextExtractionStrategy();
                        string text = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(page_number), strategy);

                        int countedSpacesPerPage = text.Split(' ').Length - 1;
                        totalNumberOfSpaces += countedSpacesPerPage;
                    }
                }
            }

            return totalNumberOfSpaces;
        }

        public static void Execute()
        {
            var context = new MLContext();

            //load data
            var trainData = context.Data.LoadFromTextFile<Object_DataSet>("C:\\Users\\rem\\source\\repos\\OJTPERFORMANCE-ASP-ML.NET-master\\ClassLibrary1\\ModelSession_2\\report_grade.csv",
                hasHeader: true, separatorChar: ',');

            var testTrainSplit = context.Data.TrainTestSplit(trainData, testFraction: 0.1);

            var pipeline = context.Transforms.Concatenate("Features", "Grade")
                .Append(context.Regression.Trainers.LbfgsPoissonRegression());

            var model = pipeline.Fit(testTrainSplit.TrainSet);

            var predictions = model.Transform(testTrainSplit.TrainSet);

            var metrics = context.Regression.Evaluate(predictions);

            var newdata = new Object_DataSet
            {
                NumberOfWords = 500,
                Grade = 70
            };

            //predict engine
            var predictFunc = context.Model.CreatePredictionEngine<Object_DataSet, Prediction>(model);

            var prediction = predictFunc.Predict(newdata);

            //Console.WriteLine($"Predicted Grade: {prediction.Prediciton}");

            //Console.ReadLine();

            var modelPath = "C:\\Users\\rem\\source\\repos\\OJTPERFORMANCE-ASP-ML.NET-master\\ClassLibrary1\\ModelSession_2\\GradePrediction.zip";

            context.Model.Save(model, trainData.Schema, modelPath);
        }

    }
}
