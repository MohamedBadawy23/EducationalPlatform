using System;

namespace EducationalPlatform.DTO
{
    public class CenterDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
        public CenterStatisticsDto Statistics { get; set; } = new CenterStatisticsDto();
    }

    public class CenterStatisticsDto
    {
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalSubjects { get; set; }
        public int TotalLessons { get; set; }
        public int TotalMaterials { get; set; }
        public int UnpaidStudents { get; set; }
    }
} 