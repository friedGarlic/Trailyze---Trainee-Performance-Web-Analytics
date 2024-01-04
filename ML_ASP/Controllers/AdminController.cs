using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ML_ASP.DataAccess.Repositories.IRepositories;
using ML_ASP.etc;
using ML_ASP.Models;
using System.CodeDom;

namespace ML_ASP.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unit;
        public AdminController(IUnitOfWork unit)
        {
            _unit = unit;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Admin()
        {
			IEnumerable<SubmissionModel> modelList = _unit.Submission.GetAll();
			List<string> options = new List<string> { "Approved", "Declined", "Remake" };

			// Create an IEnumerable<SelectListItem> using the Select method
			IEnumerable<SelectListItem> submissionList = options.Select(option => new SelectListItem
			{
				Text = option,
				Value = option
			});

            //_unit.Submission.ChangeApprovalStatus()

            

			ViewBag.submissionList = submissionList;


			return View(modelList);
        }

		[HttpPost]
		public IActionResult UpdateApprovalStatus(string approvalStatus, int id)
		{
			
			_unit.Submission.ChangeApprovalStatus(id, approvalStatus);
			_unit.Save();

			// Redirect back to the Admin view or any other view
			return RedirectToAction("Admin");
		}
	}
}
