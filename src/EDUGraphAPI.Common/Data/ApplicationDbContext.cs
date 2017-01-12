using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace EDUGraphAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Organization> Organizations { get; set; }

        public DbSet<UserTokenCache> UserTokenCacheList { get; set; }

        public DbSet<DataSyncRecord> DataSyncRecords { get; set; }

        public DbSet<ClassroomSeatingArrangements> ClassroomSeatingArrangements { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}