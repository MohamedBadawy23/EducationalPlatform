using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ServiceDto
{
   public class EInvoiceRequestModel
    {
        [JsonProperty("payment_method_id")]
        public int? PaymentMethodId { get; set; }

        [JsonProperty("customer")]
        public required CustomerModel Customer { get; set; }

        [JsonProperty("cartItems")]
        public List<CartItemModel> CartItems { get; set; }

        [JsonProperty("cartTotal")]
        public decimal CartTotal => CartItems.Sum(item => item.Price);

        [JsonProperty("currency")]
        public string Currency { get; set; } = "EGP";
        [JsonProperty("payLoad")]
        public InvoicePayload? PayLoad { get; set; }

        [JsonProperty("redirectionUrls")]
        public RedirectionUrlsModel? RedirectionUrls { get; set; }

        public class InvoicePayload
        {
            public string OrderId { get; set; }
        }

        public class CustomerModel
        {
            [JsonProperty("customer_unique_id")]
            public string? CustomerId { get; set; }

            [JsonProperty("first_name")]
            public required string FirstName { get; set; }

            [JsonProperty("last_name")]
            public required string LastName { get; set; }

            [JsonProperty("email")]
            public string? Email { get; set; }
            [JsonProperty("phone")]
            public string? Phone { get; set; }
        }

        public class CartItemModel
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("price")]
            public decimal Price { get; set; }

            
        }

        public class RedirectionUrlsModel
        {
            [JsonProperty("successUrl")]
            public string? OnSuccess { get; set; }

            [JsonProperty("failUrl")]
            public string? OnFailure { get; set; }

            [JsonProperty("pendingUrl")]
            public string? OnPending { get; set; }
        }
    }

    public class EInvoiceResponseModel
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("data")]
        public EInvoiceResponseDataModel Data { get; set; }

        public class EInvoiceResponseDataModel
        {
            [JsonProperty("url")]
            public string Url { get; set; }
            [JsonProperty("invoiceId")]
            public string InvoiceId { get; set; }
            [JsonProperty("invoiceKey")]
            public string InvoiceKey { get; set; }
        }
    }
}
