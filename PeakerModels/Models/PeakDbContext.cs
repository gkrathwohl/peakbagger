using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;


namespace peaker.Models
{
    public class PeakDbContext : DbContext
    {
        public PeakDbContext()
            : base("DefaultConnection")
        {
        }

        public static PeakDbContext Create()
        {
            return new PeakDbContext();
        }

        public DbSet<Peak> Peaks { get; set; }
        public DbSet<Athlete> Athletes { get; set; }
        public DbSet<SummitCompletion> SummitCompletions { get; set; }
        public DbSet<IndexedActivity> IndexedActivities { get; set; }
    }
}