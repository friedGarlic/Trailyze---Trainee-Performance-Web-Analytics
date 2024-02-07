using Microsoft.AspNetCore.Identity;
using ML_ASP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML_ASP.DataAccess.Repositories.IRepositories
{
	public interface IAccountRepository : IRepository<Account_Model>
	{
		public void UpdateAccount(int hrsRemain, string id);
		public void Update(Account_Model model, string id);


		public Account_Model GetFirstAndDefault();
		public double? GetRemainingHours(IdentityUser user);
		public int? GetRemainingReports(IdentityUser user);
        public string? GetImageUrl(IdentityUser user);
    }
}
