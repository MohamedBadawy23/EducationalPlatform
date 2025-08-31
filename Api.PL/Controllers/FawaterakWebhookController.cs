 
using Fawaterak.Services;
using Microsoft.AspNetCore.Mvc;
using Service.ServiceContract;
using Service.ServiceDto;
namespace Fawaterak.Controllers;

[ApiController]
[Route("api/fawaterak/webhook")]
public class FawaterakWebhookController : ControllerBase
{
    private readonly IFawaterkPaymentService _payments;

    public FawaterakWebhookController(IFawaterkPaymentService payments)
    {
        _payments = payments;
    }

    // POST: api/fawaterak/webhook-paid_json
    [HttpPost("paid_json")]
    public IActionResult WebhookPaid([FromBody] WebHookModel model)
    {
        var valid = _payments.VerifyWebhook(model);
        if (!valid) return Unauthorized();

        // Handle the payment logic here

        return Ok("got it!");
    }

    // POST: api/fawaterak/webhook-cancel
    [HttpPost("cancel")]
    public IActionResult WebhookCancel([FromBody] CancelTransactionModel model)
    {
        var valid = _payments.VerifyCancelTransaction(model);
        if (!valid) return Unauthorized();

        // Handle the cancellation logic here

        return Ok();
    }

    // POST: api/fawaterak/webhook-failed
    [HttpPost("failed")]
    public IActionResult WebhookFaild([FromBody] CancelTransactionModel model)
    {
        var valid = _payments.VerifyCancelTransaction(model);
        if (!valid) return Unauthorized();

        // Handle the failed logic here

        return Ok();
    }
} 