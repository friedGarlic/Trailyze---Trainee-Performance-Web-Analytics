﻿using ML_ASP.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML_ASP.DataAccess.Repositories.IRepositories
{
    public interface IReminderRepository : IRepository<Reminder_Model>
    {
        public void Update(int id, string reminderName, string iconClass, string iconType);
    }
}
