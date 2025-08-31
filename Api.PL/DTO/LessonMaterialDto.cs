namespace Api.PL.DTO
{
    public class LessonMaterialDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string SubjectName { get; set; }
        public string TeacherName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadDate { get; set; }
        public int DownloadCount { get; set; }
    }
}
