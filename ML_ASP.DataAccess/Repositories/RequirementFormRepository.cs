﻿using Microsoft.EntityFrameworkCore;
using ML_ASP.DataAccess.Repositories.IRepositories;
using ML_ASP.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML_ASP.DataAccess.Repositories
{
    public class RequirementFormRepository : Repository<RequirementForm_Model>, IRequirementFormRepository
    {
        private readonly DbContext _dbContext;

        public RequirementFormRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
