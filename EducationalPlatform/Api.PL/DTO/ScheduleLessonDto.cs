using System;

namespace EducationalPlatform.DTO
{
    public class ScheduleLessonDto
    {
        public int SubjectId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}