namespace Api.PL.DTO
{
    public class LessonMaterialCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string SubjectName { get; set; }
        public string TeacherName { get; set; }
        public IFormFile File { get; set; }
         
         
    }
}
