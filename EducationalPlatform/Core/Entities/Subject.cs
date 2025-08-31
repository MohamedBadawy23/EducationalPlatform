using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreProject.Enteties
{
   public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }

        public ICollection<Lesson> Lessons { get; set; } = new HashSet<Lesson>();
        public ICollection<LessonMaterial> LessonsMaterials { get; set; } = new HashSet<LessonMaterial>();
        public ICollection<Student> Students { get; set; } = new HashSet<Student>();
    }
}
