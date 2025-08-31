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
        public string Name { get; set; }
        public string Grade { get; set; }
        public string PhoneNumber { get; set; }
        public bool HasPaid { get; set; }
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;

        public int CenterId { get; set; }
        public Center Center { get; set; }
        public ICollection<Subject> Subjects { get; set; } = new HashSet<Subject>();
    }
}
