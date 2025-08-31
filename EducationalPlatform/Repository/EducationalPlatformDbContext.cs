using Core.Entities;
using CoreProject.Enteties;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryProject
{
   public class EducationalPlatformDbContext:IdentityDbContext<AppUser>
    {
        public EducationalPlatformDbContext(DbContextOptions<EducationalPlatformDbContext> options):base(options)  
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
