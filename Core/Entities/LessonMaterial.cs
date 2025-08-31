using CoreProject.Enteties;
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
        
        public string Title { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public string Description { get; set; }
        public int DownloadCount { get; set; }
        public DateTime UploadedAt { get; set; }
        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
        public Guid CenterId { get; set; }
        public Center Center { get; set; }
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }


    }
}
