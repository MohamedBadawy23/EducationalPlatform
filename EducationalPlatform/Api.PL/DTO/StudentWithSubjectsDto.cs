namespace EducationalPlatform.DTO
{
    public class StudentWithSubjectsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Grade { get; set; }
        public string PhoneNumber { get; set; }
        public bool HasPaid { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public List<StudentSubjectInfoDto> Subjects { get; set; } = new List<StudentSubjectInfoDto>();
    }

    public class StudentSubjectInfoDto
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string TeacherName { get; set; }
    }
} 