
 
using Fawaterak.Services;
using Microsoft.AspNetCore.Mvc;
using Service.ServiceContract;
using Service.ServiceDto;
namespace Fawaterak.Controllers;

[ApiController]
[Route("api/fawaterak")]
public class FawaterakPaymentsController : ControllerBase
{
    private readonly IFawaterkPaymentService _payments;

    public FawaterakPaymentsController(IFawaterkPaymentService payments)
    {
        _payments = payments;
    }

    // POST: api/fawaterak/invoices
    [HttpPost("invoices")]
    public async Task<IActionResult> CreateInvoice([FromBody] EInvoiceRequestModel request)
    {
        var data = await _payments.CreateEInvoiceAsync(request);
        if (data is null) return BadRequest();
        return Ok(data);
    }

    // GET: api/fawaterak/payment-methods
    [HttpGet("payment-methods")]
    public async Task<IActionResult> GetPaymentMethods()
    {
        var result = await _payments.GetPaymentMethods();
        if (result is null || result.Count == 0) return NoContent();
        return Ok(result);
    }

    // POST: api/fawaterak/pay
    [HttpPost("pay")]
    public async Task<IActionResult> Pay([FromBody] EInvoiceRequestModel invoice)
    {
        var result = await _payments.GeneralPay(invoice);
        if (result is null) return BadRequest();
        return Ok(result);
    }

    // GET: api/fawaterak/iframe-hash
    [HttpGet("iframe-hash")]
    public IActionResult IFrameHash(string domain)
    {
        var result = _payments.GenerateHashKeyForIFrame(domain);
        return Ok(result);
    }
}
