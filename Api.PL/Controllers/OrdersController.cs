 
using Fawaterak.Services;
using Microsoft.AspNetCore.Mvc;
using Service.ServiceContract;
using Service.ServiceDto;

namespace Fawaterak.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IFawaterkPaymentService _fawaterakPaymentService;

    public OrdersController(IFawaterkPaymentService fawaterakPaymentService)
    {
        _fawaterakPaymentService = fawaterakPaymentService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderDto createOrderDto)
    {
        #region Pseudo Code
        // 1. Validate the ShoppingCartId && Payment Method Id
        // 2. Retrieve the shopping cart items from the database or cache
        // 3. validate the items in the cart 
        // 4. Calculate the total price of the items in the cart
        // 5. create an order record in the database with status "Pending"
        // 6. Create the invoice using FawaterakPaymentService
        // 7. Save the invoice id in the order record & order id in the invoice payload
        // 8. Return the order details along with the invoice link to the client 
        #endregion

        #region Create the invoice
        var invoice = new EInvoiceRequestModel()
        {
            Currency = "EGP",
            PaymentMethodId = createOrderDto.PaymentMethodId,
            Customer = new EInvoiceRequestModel.CustomerModel
            {
                // from current logged-in user
                FirstName = "John",
                LastName = "Doe",
                Email = "user@gmail.com",
                CustomerId = "current-user-id",
                Phone = "01122445555"
            },
            CartItems =
            [
                new EInvoiceRequestModel.CartItemModel
                {
                    Name = "item-1",
                    
                    Price = 100.0m
                },
                new EInvoiceRequestModel.CartItemModel
                {
                    Name = "plan-2",
                   
                    Price = 50.0m
                }
            ],
            PayLoad = new EInvoiceRequestModel.InvoicePayload
            {
                OrderId = "order-id-001"
            },
            RedirectionUrls = new EInvoiceRequestModel.RedirectionUrlsModel
            {
                OnSuccess = "https://domain-of-my-project.com/success",
                OnFailure = "https://domain-of-my-project.com/failure",
                OnPending = "https://domain-of-my-project.com/pending"
            }
        };
        #endregion

        // Call Fawaterak to create the invoice
        var invoiceResult = await _fawaterakPaymentService.GeneralPay(invoice);

        return Ok(new { Message = "Order created successfully", invoice, result = (object)invoiceResult });
    }
}