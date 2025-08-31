using Core.Entities;

namespace Api.PL.DTO
{
    public class ScheduleDto
    {
        public string SubjectName { get; set; }
        public string TeacherName { get; set; }
        public string RoomNmae { get; set; }
        
        public Statues Statue { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int StudentCount { get; set; }
    }
}
