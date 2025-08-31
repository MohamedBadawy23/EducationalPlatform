using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;
using System;

namespace BookingSystem.Helper
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Console.WriteLine($"Sending email to: {email}\nSubject: {subject}\nMessage: {htmlMessage}");
            return Task.CompletedTask;
        }
    }
} 