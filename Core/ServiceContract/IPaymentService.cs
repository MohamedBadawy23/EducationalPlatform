using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ServiceContract
{
  public  interface IPaymentService
    {
        Task<(Enrollment Enrollment, string RedirectUrl)> ProcessPaymentAsync(int enrollmentId, string paymentMethod, decimal amount);
        Task<Enrollment> UpdateOrderSuccess(string specialReference);
        Task<Enrollment> UpdateOrderFailed(string specialReference);
        string ComputeHmacSHA512(string data, string secret);
    }
}
