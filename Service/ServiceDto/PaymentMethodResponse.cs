using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ServiceDto
{
   public class PaymentMethodResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("data")]
        public List<PaymentMethod> Data { get; set; }

        public class PaymentMethod
        {
            public int Id { get; set; }
            [JsonProperty("paymentId")]
            public int PaymentId { get; set; }

            [JsonProperty("name_en")]
            public string NameEn { get; set; }

            [JsonProperty("name_ar")]
            public string NameAr { get; set; }

            [JsonProperty("redirect")]
            public string Redirect { get; set; }

            [JsonProperty("logo")]
            public string Logo { get; set; }

            public static explicit operator int(PaymentMethod v)
            {
                throw new NotImplementedException();
            }
        }
    }
}
