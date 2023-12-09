using Microsoft.EntityFrameworkCore;
using ML_ASP.Models;

namespace ML_ASP.Data
{
    public class ML_DBContext : DbContext
    {
        public ML_DBContext(DbContextOptions<ML_DBContext> options) : base(options)
        { }

        public DbSet<Account_Model> Accounts { get; set; }
        public DbSet<Trainee_Model> Trainees { get; set; }
    }
}
