using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace ML_ASP.Models
{
    public class SubmissionModel
    {
        [Key]
        public int Id { get; set; }

        public string SubmissionUserId { get; set; }

        public string? FileName { get; set; }

        public string? Name { get; set; }

        public DateTime Date { get; set; }

        public string? DueStatus { get; set; }

        public string? ApprovalStatus { get; set; }

        public string? Details { get; set; }

        public string? Grade { get; set; }
    }
}
