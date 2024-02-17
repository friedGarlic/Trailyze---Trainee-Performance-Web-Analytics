using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ML_ASP.Models;
using ML_ASP.Models.Models;

namespace ML_ASP.DataAccess
{
    public class ApplicationDBContext : IdentityDbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        { }

        public DbSet<Account_Model> Accounts { get; set; }
        public DbSet<Trainee_Model> Trainees { get; set; }
        public DbSet<SubmissionModel> Submissions { get; set; }
        public DbSet<LogModel> Logs { get; set; }
        public DbSet<Reminder_Model> Reminders { get; set; }
    }
}
