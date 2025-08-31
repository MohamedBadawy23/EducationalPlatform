using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreProject.Enteties;

namespace Core.Entities
{
   public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        public string PaymentMethod { get; set; }

        public string TransactionId { get; set; }

        public string Status { get; set; } = "Pending";

        public int EnrlollmentId { get; set; }
        public Enrollment Enrollment { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }

        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
    }
}
