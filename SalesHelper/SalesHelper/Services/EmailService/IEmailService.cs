using SalesHelper.Models.EmailSettings;

namespace SalesHelper.Services.EmailService
{
    public interface IEmailService
    {
        MailSettings GetMailSettings();
        Task SendEmailAsync();
    }
}
