using ML_ASP.DataAccess.Repositories.IRepositories;
using ML_ASP.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML_ASP.DataAccess.Repositories
{
    public class ReminderRepository : Repository<Reminder_Model>, IReminderRepository
    {
        private readonly ApplicationDBContext _dbContext;


        public ReminderRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(int id, string reminderName, string iconClass, string iconType)
        {
            var objFromDb = _dbContext.Reminders.FirstOrDefault(u => u.Id == id);
        }

    }
}
