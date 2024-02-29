using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using SalesHelper.Data;
using SalesHelper.Models.EmailSettings;
using System.Security.Claims;

namespace SalesHelper.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EmailService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;

        }
        public void SaveMailSettings(MailSettings mailSettings)
        {
            try
            {
                _context.MailSettings.Add(mailSettings);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public MailSettings GetMailSettings()
        {
            var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var mailSettings = _context.MailSettings.Where(ms => ms.CreatedByUserId == userId).FirstOrDefault()!;
            if (mailSettings == null)
            {
                throw new Exception("MailSettings_Not_Found");
            }
            return mailSettings;
        }

        public Task SendEstimateRequestEmail(string to, string subject, string body, string attachmentName, byte[]? pdfBytes)
        {
            try
            {
                var _mailSettings = GetMailSettings();

                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_mailSettings.Sender));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.TextBody = body;

                // if attachment is not null, add it to the email using mailkit attachment
                if (pdfBytes != null)
                {
                    bodyBuilder.Attachments.Add(attachmentName, pdfBytes);
                }

                email.Body = bodyBuilder.ToMessageBody();

                using var smtp = new SmtpClient();
                smtp.Connect(_mailSettings.Server, _mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Sender, _mailSettings.Password);
                smtp.Send(email);
                smtp.Disconnect(true);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
