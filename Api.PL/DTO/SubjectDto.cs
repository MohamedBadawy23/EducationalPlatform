using System.ComponentModel.DataAnnotations.Schema;

namespace Api.PL.DTO
{
    public class SubjectDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        //public string TeacherName { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
    }
}
