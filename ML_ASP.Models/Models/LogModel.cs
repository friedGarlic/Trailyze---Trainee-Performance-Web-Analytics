using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML_ASP.Models.Models
{
    public class LogModel
    {
        public int Id { get; set; }

        public string LogId { get; set; }

        public DateTime DateTime { get; set; }

        public string? Log { get; set; }

        public string? ImageUrl { get; set; }

        public string? LogImageUrl { get; set; }

        [DisplayName("Full Name")]
        public string FullName { get; set; }
    }
}
