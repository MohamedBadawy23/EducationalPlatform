using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ServiceDto
{
   public class CardPaymentData
    {
        [JsonProperty("redirectTo")] public string RedirectTo { get; set; }
    }
}
