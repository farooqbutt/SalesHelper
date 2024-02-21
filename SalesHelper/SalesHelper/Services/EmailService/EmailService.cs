using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using SalesHelper.Data;
using SalesHelper.Models.EmailSettings;
using System.Security.Claims;

namespace SalesHelper.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly MailSettings _mailSettings;
        public EmailService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mailSettings = GetMailSettings();

        }
        public MailSettings GetMailSettings()
        {
            var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var mailSettings = _context.MailSettings.Where(ms => ms.CreatedByUserId == userId).FirstOrDefault()!;
            return mailSettings;
        }
        public Task SendEmailAsync()
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_mailSettings.Sender));
                email.To.Add(MailboxAddress.Parse("To"));
                email.Subject = "Subject";
                email.Body = new TextPart(TextFormat.Html) { Text = "Body" };

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
