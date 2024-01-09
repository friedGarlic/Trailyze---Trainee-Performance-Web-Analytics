using ML_ASP.DataAccess.Repositories.IRepositories;
using ML_ASP.Models;
using ML_ASP.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML_ASP.DataAccess.Repositories
{
    public class LogRepository : Repository<LogModel>, ILogRepository
    {
        private ApplicationDBContext _dbContext;

        public LogRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(LogModel logModel)
        {
            _dbContext.Update(logModel);
        }
    }
}
