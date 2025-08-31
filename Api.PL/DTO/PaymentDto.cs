using System;

namespace EducationalPlatform.DTO
{
    public class PaymentDto
    {
        public int StudentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Description { get; set; }
    }
}