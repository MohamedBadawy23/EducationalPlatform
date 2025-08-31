using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Service.ServiceDto
{
    public class WebhookPayload
    {
        [JsonPropertyName("OrderId")]
        public string? OrderId { get; set; }
    }

    public class WebHookModel
    {
        [JsonPropertyName("invoice_id")]
        public long InvoiceId { get; set; }
        [JsonPropertyName("invoice_key")]
        public string InvoiceKey { get; set; }
        [JsonPropertyName("hashKey")]
        public string HashKey { get; set; }
        [JsonPropertyName("payment_method")]
        public string PaymentMethod { get; set; }
        [JsonPropertyName("invoice_status")]
        public string InvoiceStatus { get; set; }
        [JsonPropertyName("pay_load")]
        public string? PayloadString { get; set; }
        [JsonPropertyName("pay_load")]
        public WebhookPayload? Payload { get; set; }
    }

    public class CancelTransactionModel
    {
        [JsonPropertyName("hashKey")]
        public string HashKey { get; set; }
        [JsonPropertyName("referenceId")]
        public string ReferenceId { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("paymentMethod")]
        public string PaymentMethod { get; set; }
        [JsonPropertyName("pay_load")]
        public object? PayLoad { get; set; }
    }

    public class FaliledWebHook
    {
        [FromForm(Name = "invoice_id")]
        public long InvoiceId { get; set; }

        [FromForm(Name = "invoice_key")]
        public string InvoiceKey { get; set; }

        [FromForm(Name = "errorMessage")]
        public string ErrorMessage { get; set; }
    }
}
