using Microsoft.EntityFrameworkCore;
using ML_ASP.Models;

namespace ML_ASP.DataAccess
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        { }

        public DbSet<Account_Model> Accounts { get; set; }
        public DbSet<Trainee_Model> Trainees { get; set; }
        public DbSet<SubmissionModel> Submissions { get; set; }
    }
}
