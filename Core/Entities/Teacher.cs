using CoreProject.Enteties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Teacher
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Specialist { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public double? Rating { get; set; }
        public Guid CenterId { get; set; }
        public Center Center { get; set; }
      
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }

        public ICollection<Student> Students { get; set; } = new HashSet<Student>();

    }
}
