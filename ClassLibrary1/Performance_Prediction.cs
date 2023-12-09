using Microsoft.ML.Data;

namespace ML_net
{
    public class Performance_Prediction
    {
        [ColumnName("Score")]
        public float PerformancePrediciton_Score { get; set; }
    }
}