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
        public string? Description { get; set; }
        public decimal Price { get; set; }
        // public string Description { get; set; }
        //public ICollection<Teacher> Teachers { get; set; } = new HashSet<Teacher>();

        public Guid CenterId { get; set; }
       public Center Center { get; set; }

       // public ICollection<Lesson> Lessons { get; set; }=new HashSet<Lesson>();
      //  public ICollection<LessonMaterial> LessonsMaterials { get; set; } =new HashSet<LessonMaterial>();
     //   public ICollection<StudentSubject> StudentSubjects { get; set; }= new HashSet<StudentSubject>();
      
    }
}
