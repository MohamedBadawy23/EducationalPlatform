using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ServiceDto
{
   public class CreateOrderDto
    {
        public string CartId { get; set; }
        public int PaymentMethodId { get; set; }
    }
}
