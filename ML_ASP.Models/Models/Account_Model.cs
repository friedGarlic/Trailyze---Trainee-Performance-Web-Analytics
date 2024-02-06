using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ML_ASP.Models
{
    public class Account_Model : IdentityUser
    {
        [Range(18,50)]
        public int Age { get; set; }

        [DisplayName("Full Name")]
        public string FullName { get; set; }

        //for viewing
        public int? WeeklyReportRemaining { get; set; }

        public double? HoursRemaining { get; set; }
        public double? HoursCompleted { get; set; }
        public string? ImageUrl { get; set; }
    }
}
