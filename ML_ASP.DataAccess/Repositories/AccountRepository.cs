using Microsoft.AspNetCore.Identity;
using ML_ASP.DataAccess.Repositories.IRepositories;
using ML_ASP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML_ASP.DataAccess.Repositories
{
	public class AccountRepository : Repository<Account_Model>, IAccountRepository
	{
		private ApplicationDBContext _dbContext;

		public AccountRepository(ApplicationDBContext dbContext) : base(dbContext)
		{
			_dbContext = dbContext;
		}

        public Account_Model GetFirstAndDefault()
        {
			return _dbContext.Accounts.FirstOrDefault();
        }

		public void Update(Account_Model model, string id)
		{
            var objFromDb = _dbContext.Accounts.FirstOrDefault(u => u.Id == id);
            if (objFromDb != null)
            {
                objFromDb.HoursRemaining = model.HoursRemaining;
				objFromDb.WeeklyReportRemaining = model.WeeklyReportRemaining;
            }
		}

		public double? GetRemainingHours(IdentityUser user)
		{
            var objFromDb = _dbContext.Accounts.FirstOrDefault(u => u.Id == user.Id);

			var remain = objFromDb.HoursRemaining;

            return remain;
        }

        public int? GetRemainingReports(IdentityUser user)
        {
            var objFromDb = _dbContext.Accounts.FirstOrDefault(u => u.Id == user.Id);

            var remain = objFromDb.WeeklyReportRemaining;

            return remain;
        }
    }
}
