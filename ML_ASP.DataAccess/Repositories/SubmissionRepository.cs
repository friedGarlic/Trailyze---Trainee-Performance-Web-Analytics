using ML_ASP.DataAccess.Repositories.IRepositories;
using ML_ASP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ML_ASP.DataAccess.Repositories
{
	public class SubmissionRepository : Repository<SubmissionModel>, ISubmissionRepository
	{
		private ApplicationDBContext _dbContext;

		public SubmissionRepository(ApplicationDBContext dbContext) : base(dbContext)
		{
			_dbContext = dbContext;
		}

		public void Update(SubmissionModel model)
		{
			_dbContext.Update(model);
		}
	}
}
