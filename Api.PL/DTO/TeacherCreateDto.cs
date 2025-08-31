namespace Api.PL.DTO
{
    public class TeacherCreateDto
    {

        public string Name { get; set; }  
        public string SubjectName { get; set; } = null!; // إدخال باسم المادة
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Specialist { get; set; } = null!;
       // public string Description { get; set; } = null!;

    }
}
