﻿using ML_ASP.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML_ASP.Models.ViewModel
{
    public class RequirementVM
    {
        public AccountInfo_Model AccountInfoModel {  get; set; }

        public string FileName1 { get; set; }
        public string FileName2 { get; set; }
        public string FileName3 { get; set; }

        public bool IsSubmittedFile1 { get; set; }
        public bool IsSubmittedFile2 { get; set; }
        public bool IsSubmittedFile3 { get; set; }
    }
}
