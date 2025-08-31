using Core.Entities;
using CoreProject.Enteties;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace RepositoryProject
{
    public class EducationalPlatformDbContext : IdentityDbContext<AppUser>
    {
        public EducationalPlatformDbContext(DbContextOptions<EducationalPlatformDbContext> options) : base(options)
        {

        }

        public DbSet<Center> Centers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<LessonBooking> LessonBookings { get; set; }
        public DbSet<LessonMaterial> LessonMaterials { get; set; }
        public DbSet<Message> messages { get; set; }
        public DbSet<Schedule> schedules { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<OtpEntry> OtpEntries { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Payment> Payments { get; set; }


        //   public DbSet<Center> Centers { get; set; }
        //   public DbSet<Student> Students { get; set; }
        //   public DbSet<LessonBooking> LessonBookings { get; set; }
        // //  public DbSet<StudentSubject> StudentSubjects { get; set; }
        ////   public DbSet<Message> Messages { get; set; }
        //   public DbSet<Subject> Subjects { get; set; }
        //   public DbSet<Teacher> Teachers { get; set; }
        // //  public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
