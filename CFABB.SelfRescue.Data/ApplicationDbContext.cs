using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace CFABB.SelfRescue.Data {
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDBContext {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {
        }


        #region DBSets
        public DbSet<Entry> Entries { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public DbSet<EntryCategory> EntryCategories { get; set; }
        public DbSet<EntryTag> EntryTags { get; set; }
        #endregion 

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            base.OnConfiguring(optionsBuilder);
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            builder.Entity<EntryCategory>().HasKey(c => new { c.EntryId, c.CategoryId });
            builder.Entity<EntryTag>().HasKey(c => new { c.EntryId, c.TagId });
        }

        /// <inheritdoc />
        public int SaveChanges(string userName) {
            return base.SaveChanges();
        }

        /// <inheritdoc />
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken, string userName) {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
