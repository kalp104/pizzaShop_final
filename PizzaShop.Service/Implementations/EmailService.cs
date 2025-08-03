using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations;

public class EmailService : IEmailService
{
    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        try
        {
            using var mail = new MailMessage();
            mail.From = new MailAddress("test.dotnet@etatvasoft.com", "Kalp pandya");
            mail.To.Add(toEmail);
            mail.Subject = subject;
            mail.Body = message;
            mail.IsBodyHtml = true;

            using var smtp = new SmtpClient("mail.etatvasoft.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("test.dotnet@etatvasoft.com", "P}N^{z-]7Ilp"),
                EnableSsl = true,
            };

            await smtp.SendMailAsync(mail);
        }
        catch (SmtpException ex)
        {
            Console.WriteLine($"SMTP error: {ex.Message}, StatusCode: {ex.StatusCode}, InnerException: {ex.InnerException?.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Email sending failed: {ex.Message}, InnerException: {ex.InnerException?.Message}");
            throw;
        }
    }
}
