using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ServiceContract
{
   public interface IEmailSender
    {
        Task   SendEmailAsync(string toEmail, string subject, string body);
    }
}
