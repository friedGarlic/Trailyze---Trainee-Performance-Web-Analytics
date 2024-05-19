using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML_ASP.Models.ViewModel
{
    public class RequirementVM
    {
        public string FileName1 { get; set; }
        public string FileName2 { get; set; }
        public string FileName3 { get; set; }

        public bool IsSubmittedFile1 { get; set; }
        public bool IsSubmittedFile2 { get; set; }
        public bool IsSubmittedFile3 { get; set; }
    }
}
