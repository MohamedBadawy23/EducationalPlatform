using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ServiceDto
{
   public class BasePaymentDataResponse
    {
        [JsonProperty("invoice_id")] public string InvoiceId { get; set; }

        [JsonProperty("invoice_key")] public string InvoiceKey { get; set; }
    }
}
