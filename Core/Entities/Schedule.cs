using CoreProject.Enteties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
   public class Schedule
    {
        public int Id { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        
        public Statues Statue { get; set; }
        public int SubjectId { get; set; }  
        public Subject Subject { get; set; }
        public Guid CenterId { get; set; }
        public Center Center { get; set; }
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }

        public int RoomId { get; set; }
        public Room Room { get; set; }

        public ICollection<Student> Students { get; set; } = new HashSet<Student>();
    }
}
