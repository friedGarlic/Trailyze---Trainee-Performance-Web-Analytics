using ML_ASP.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML_ASP.Models.ViewModel
{
	public class SubmissionVM
	{
		public IEnumerable<SubmissionModel> SubmissionList { get; set; }

		public string AccountName { get; set; }

		public string TimeLog { get; set; } = "Timed Out";

		public IEnumerable<LogModel> LogList { get; set; }
	}
}
