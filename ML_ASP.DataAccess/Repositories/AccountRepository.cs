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

		public void Update(Account_Model model, string id) //update Image url
		{
            var objFromDb = _dbContext.Accounts.FirstOrDefault(u => u.Id == id);
            if (objFromDb != null)
            {
                objFromDb.ImageUrl = model.ImageUrl;
            }
		}

		public void UpdateAccount(int hrsRemain, string id)
		{
			var objFromDb = _dbContext.Accounts.FirstOrDefault(u => u.Id == id);
			if (objFromDb != null)
			{
				objFromDb.HoursRemaining = hrsRemain;
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

        public string? GetImageUrl(IdentityUser user)
        {
            var objFromDb = _dbContext.Accounts.FirstOrDefault(u => u.Id == user.Id);

            var remain = objFromDb.ImageUrl;

            return remain;
        }
    }
}
