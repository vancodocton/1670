using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace WebApp.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Trainer> Trainers { get; set; }

        public DbSet<Trainee> Trainees { get; set; }

        public DbSet<CourseCategory> CourseCategories { get; set; }

        public DbSet<Course> Courses { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
            .HasRequired(t => t.Trainer)
            .WithRequiredPrincipal(t => t.User)
            .WillCascadeOnDelete(true);

            modelBuilder.Entity<ApplicationUser>()
            .HasRequired(t => t.Trainee)
            .WithRequiredPrincipal(t => t.User)
            .WillCascadeOnDelete(true);
        }
    }
}