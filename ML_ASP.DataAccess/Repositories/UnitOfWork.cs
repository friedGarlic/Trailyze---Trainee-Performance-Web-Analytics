using Microsoft.EntityFrameworkCore.Query.Internal;
using ML_ASP.DataAccess.Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML_ASP.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDBContext _dbContext;

        public UnitOfWork(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
            Submission = new SubmissionRepository(_dbContext);
            Account = new AccountRepository(_dbContext);
            Log = new LogRepository(_dbContext);
            Reminder = new ReminderRepository(_dbContext);
        }

        public ISubmissionRepository Submission { get; private set; }
        public IAccountRepository Account { get; private set; }
        public ILogRepository Log { get; private set; }
        public IReminderRepository Reminder { get; private set; }
        public IQRRepository QR { get; private set; }

        public void Save()
        {
            _dbContext.SaveChanges();
        }
    }
}
