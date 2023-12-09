using Microsoft.ML;
using ML_net;

namespace Setup
{
    public class Setup
    {
        public static void Main(string[] args)
        {
            var context = new MLContext();

            //load data
            var trainData = context.Data.LoadFromTextFile<Performance_DataSet>("./StudentDataSet.csv",
                hasHeader: true, separatorChar: ',');

            var testTrainSplit = context.Data.TrainTestSplit(trainData, testFraction: 0.1);

            //build model
            var pipeline = context.Transforms.Concatenate("Features", "PerformanceScore")
                .Append(context.Regression.Trainers.LbfgsPoissonRegression());

            var model = pipeline.Fit(testTrainSplit.TrainSet);

            //evaluate
            var predictions = model.Transform(testTrainSplit.TrainSet);

            var metrics = context.Regression.Evaluate(predictions);

            Console.WriteLine($"R^2 - {metrics.RSquared}");

            //predict

            var newdata = new Performance_DataSet
            {
                CompletionTime = 30,
                PerformanceScore = 86,
                FeedbackScore = 3.2f,
                EmployeeID = 1
            };

            //predict engine
            var predictFunc = context.Model.CreatePredictionEngine<Performance_DataSet, Performance_Prediction>(model);

            var prediction = predictFunc.Predict(newdata);

            Console.WriteLine($"Prediction - {prediction.PerformancePrediciton_Score}");

            Console.ReadLine();
        }
    }
}