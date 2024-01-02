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

        public void Save()
		{
			_dbContext.SaveChanges();
		}

		public void Update(Account_Model model)
		{
			_dbContext.Update(model);
		}
	}
}
