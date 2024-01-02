namespace ML_ASP.Models
{
    public class Trainee_Model
    {
        public Guid Id { get; set; }
        
        public int TrainingSequence { get; set; }

        public int CompletionTime { get; set; }

        public int PerformanceScore { get; set; }

        public float FeedbackScore { get; set; }

        public int TraineeID { get; set; }
    }
}
