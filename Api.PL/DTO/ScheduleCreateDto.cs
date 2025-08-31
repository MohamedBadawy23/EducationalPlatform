using Core.Entities;

namespace Api.PL.DTO
{
    public class ScheduleCreateDto
    {
        public string TeacherName { get; set; }
        public string SubjectName { get; set; }
       public Statues Statues { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string RoomNmae {  get; set; }
    }
}
