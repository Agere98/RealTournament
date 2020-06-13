using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace RealTournament.Services
{
    public class EmailSender : IEmailSender
    {
        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public AuthMessageSenderOptions Options { get; }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(Options, subject, message, email);
        }

        public Task Execute(AuthMessageSenderOptions options, string subject, string message, string email)
        {
            var credentials = new NetworkCredential(options.Email, options.Password);
            var mail = new MailMessage
            {
                From = new MailAddress("noreply@realtournament.com"),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };
            mail.To.Add(new MailAddress(email));
            var client = new SmtpClient()
            {
                Port = options.Port,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Host = options.Host,
                EnableSsl = true,
                Credentials = credentials
            };
            return client.SendMailAsync(mail);
        }
    }
}
