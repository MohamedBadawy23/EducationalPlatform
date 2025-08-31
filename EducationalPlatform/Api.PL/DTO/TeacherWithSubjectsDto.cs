namespace EducationalPlatform.DTO
{
    public class TeacherWithSubjectsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Specialist { get; set; }
        public List<string> Subjects { get; set; } = new List<string>();
    }
} 