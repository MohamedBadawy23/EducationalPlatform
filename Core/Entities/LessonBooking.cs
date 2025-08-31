using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class LessonBooking
    {
        public int Id { get; set; }
        public DateTime BookingDate { get; set; }
        public bool isPaid { get; set; }
        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }  
    }
}
