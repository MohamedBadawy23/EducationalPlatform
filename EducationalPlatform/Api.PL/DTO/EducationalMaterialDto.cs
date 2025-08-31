using System;
using Microsoft.AspNetCore.Http;

namespace EducationalPlatform.DTO
{
    public class EducationalMaterialDto
    {
        public int SubjectId { get; set; }
        public string Title { get; set; }
        public IFormFile File { get; set; }
    }
}