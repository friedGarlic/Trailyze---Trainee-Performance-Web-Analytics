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

        //remaining
        public int? HoursRemaining { get; set; }
        public int? MinutesRemaining { get; set; }
        public int? SecondsRemaining { get; set; }

        //completed
        public int? HoursCompleted { get; set; }
        public int? MinutesCompleted { get; set; }
        public int? SecondsCompleted { get; set; }

        //total
        public TimeSpan TotalTime { get; set; }

        //profile
        public string? ImageUrl { get; set; }
    }
}
