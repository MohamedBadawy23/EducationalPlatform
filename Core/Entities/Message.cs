using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime SentDate { get; set; }
        public MessageType MessageType { get; set; }
        public MessageTemplate Template { get; set; }
        public string Body { get; set; }
        public bool IsRead { get; set; }
        public bool IsSent { get; set; }
        public int UserId { get; set; }
        public AppUser AppUser { get; set; }

        public Guid CenterId { get; set; }
        public Center Center { get; set; }

        
        public ICollection <Student> Students { get; set; }= new HashSet<Student>();

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }
    }
}
