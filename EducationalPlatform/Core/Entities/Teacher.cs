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
        public string Specialist { get; set; }
        public int CenterId { get; set; }
        public Center Center { get; set; }
        public ICollection<Subject> Subjects { get; set; } = new HashSet<Subject>();
    }
}
