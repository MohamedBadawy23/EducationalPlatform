namespace EducationalPlatform.DTO
{
    public class UploadMaterialDto
    {
        public int SubjectId { get; set; }
        public int TeacherId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile FilePath { get; set; }
    }
}
