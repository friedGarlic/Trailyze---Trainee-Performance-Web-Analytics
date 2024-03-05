﻿using ML_ASP.Models.Models;
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

		public IEnumerable<Account_Model> AccountList { get; set; }

        public IEnumerable<Reminder_Model> ReminderList { get; set; }

		public IEnumerable<LogModel> LogList { get; set; }

        public LogModel LogModel { get; set; }

		public string CurrentUserId{ get; set; }

        public string AccountName { get; set; }

		public string TimeLog { get; set; }


		public string Prediction { get; set; }

		public bool IsMultipleFile { get; set; }

		public List<string> GradeList { get; set; }

		public string SearchQuery { get; set; }
	}
}
