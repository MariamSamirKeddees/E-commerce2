using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;

namespace E_Commerce.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IConfiguration config, ILogger<EmailSender> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var smtpConfig = _config.GetSection("Smtp").Get<SmtpConfig>();
            if (smtpConfig == null || string.IsNullOrWhiteSpace(smtpConfig.Host))
            {
                _logger.LogWarning("SMTP not configured. Skipping email to {Email}. Subject: {Subject}", email, subject);
                return;
            }

            var client = new SmtpClient(smtpConfig.Host, smtpConfig.Port)
            {
                Credentials = new NetworkCredential(smtpConfig.Username, smtpConfig.Password),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpConfig.Username),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);

        }
       
    }
}
