using System;

namespace EducationalPlatform.DTO
{
    public class LessonScheduleDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string SubjectName { get; set; }
        public string TeacherName { get; set; }
    }
} 