using CoreProject.Enteties;

namespace Api.PL.DTO
{
    public class TeacherDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public string Specialist { get; set; } = null!;
        public bool IsActive { get; set; }  
        public double? Rating { get; set; }
        public int StudentCount { get; set; }

    }
}
