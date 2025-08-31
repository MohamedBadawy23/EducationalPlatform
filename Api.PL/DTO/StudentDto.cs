namespace Api.PL.DTO
{
    public class StudentDto
    {
       
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public bool HasPaid { get; set; }
        public List<SubjectIdInfoDto> Subjects { get; set; }

    }
}
