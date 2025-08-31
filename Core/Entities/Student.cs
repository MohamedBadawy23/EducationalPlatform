using CoreProject.Enteties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string? Grade { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public bool? HasPaid { get; set; }
        public string? Email { get; set; } // Nullable to allow for students without email addresses
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;

        public Guid CenterId {  get; set; }
        public Center Center { get; set; }
        public ICollection<Subject> Subjects { get; set; } = new HashSet<Subject>();

        public string UserId { get; set; }
        public AppUser User { get; set; }

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }
        // public ICollection<Schedule> Schedules { get; set; }= new HashSet<Schedule>();

    }
}
