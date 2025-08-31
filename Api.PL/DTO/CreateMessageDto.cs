using Core.Entities;

namespace Api.PL.DTO
{
    public class CreateMessageDto
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public MessageType MessageType { get; set; }
        public MessageTemplate Template { get; set; }
        public string TeacherName { get; set; }
        public string StudentName { get; set; }
        public List<int>StudentIds { get; set; }

    }
}
