using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

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

        public DbSet<CourseTrainee> CourseTrainees { get; set; }

        public DbSet<CourseCategory> CourseCategories { get; set; }

        public DbSet<Course> Courses { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}