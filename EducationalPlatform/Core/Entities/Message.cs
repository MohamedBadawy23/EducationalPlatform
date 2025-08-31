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
        public DateTime SentDate { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public string UserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
