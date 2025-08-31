namespace Api.PL.DTO
{
    public class MessageDto
    {
       public string Title { get; set; }
        public string Body { get; set; }
        public string Type { get; set; }
        public string Template { get; set; }
        public string TeacherName { get; set; }
        public string StudentName { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsSent { get; set; }
        public int RecipientCount { get; set; }
        public List<string> RecipientNames { get; set; }
    }
}
