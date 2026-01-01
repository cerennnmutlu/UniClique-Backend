using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using UniCliqueBackend.Application.Interfaces.Services;

namespace UniCliqueBackend.Persistence.Email
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public SmtpEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var smtp = new SmtpClient(
                _configuration["Email:SmtpHost"],
                int.Parse(_configuration["Email:SmtpPort"]!)
            )
            {
                Credentials = new NetworkCredential(
                    _configuration["Email:Username"],
                    _configuration["Email:Password"]
                ),
                EnableSsl = true
            };

            var mail = new MailMessage(
                _configuration["Email:From"]!,
                to,
                subject,
                body
            );

            await smtp.SendMailAsync(mail);
        }
    }
}
