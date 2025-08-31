using System;

namespace EducationalPlatform.DTO
{
    public class ScheduleDetailsDto
    {
        public string SubjectName { get; set; }
        public string TeacherName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Room {  get; set; }
        public string Statues { get; set; }
        public int StudentCount { get; set; }
    }
}