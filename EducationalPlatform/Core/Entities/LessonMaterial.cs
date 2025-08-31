using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class LessonMaterial
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public string Description { get; set; }
        public DateTime UploadedAt { get; set; }
        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }
    }
}
