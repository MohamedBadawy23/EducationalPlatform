using Service.ServiceDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Service.ServiceDto.EInvoiceResponseModel;
using static Service.ServiceDto.PaymentMethodResponse;

namespace Service.ServiceContract
{
  public  interface IFawaterkPaymentService
    {
        // Create EInvoice Link
        Task<EInvoiceResponseDataModel?> CreateEInvoiceAsync(EInvoiceRequestModel eInvoice);

        // Payment Integration
        Task<IList<PaymentMethod>?> GetPaymentMethods();
        Task<BasePaymentDataResponse?> GeneralPay(EInvoiceRequestModel invoice);

        // WebHook Verification
        bool VerifyWebhook(WebHookModel webHook);
        bool VerifyCancelTransaction(CancelTransactionModel cancelTransaction);
        bool VerifyApiKeyTransaction(string apiKey);

        // HashKey
        string GenerateHashKeyForIFrame(string domain);
    }
}
