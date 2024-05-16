using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML_ASP.Models.Models
{
    public class RequirementFile_Model
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        public string File_ApplicationForm { get; set; }

        public string File_ContractAgreement {  get; set; }

        public string File_TimeSheet { get; set; }

        public string File_ParentalConsent { get; set; }

        public string File_ProgressReport { get; set; }
    }
}
