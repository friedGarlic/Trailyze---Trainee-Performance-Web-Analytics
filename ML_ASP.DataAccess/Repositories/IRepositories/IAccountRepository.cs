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
		public void Update(Account_Model model);
		public void Save();
	}
}
