using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ServiceDto
{
   public class BasePaymentResponse
    {
       
    [JsonProperty("status")] public string Status { get; set; }
    }
}
 
