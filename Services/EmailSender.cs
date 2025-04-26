using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using PraksaApp.Models;

namespace PraksaApp.Services
{
    public class EmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
            //Console.WriteLine($"Email Settings - From: {emailSettings.Value}, Username: {_emailSettings.Username}");
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {

            if (string.IsNullOrEmpty(_emailSettings.Username))
                throw new Exception("ERROR: EmailSettings.Username is null or empty!");
            if (string.IsNullOrEmpty(toEmail))
                throw new Exception("ERROR: toEmail is null or empty!");

            try
            {
                var mail = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.Username, "PraksaApp Notifier"),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };
                mail.To.Add(toEmail);

                using var smtp = new SmtpClient
                {
                    Host = _emailSettings.Host,
                    Port = _emailSettings.Port,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                };
                await smtp.SendMailAsync(mail);


                await smtp.SendMailAsync(mail);

                Console.WriteLine($"Email successfully sent to {toEmail}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }

        }

    }
}
