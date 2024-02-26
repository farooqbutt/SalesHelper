using SalesHelper.Models.EmailSettings;

namespace SalesHelper.Services.EmailService
{
    public interface IEmailService
    {
        void SaveMailSettings(MailSettings mailSettings);
        MailSettings GetMailSettings();
        Task SendEstimateRequestEmail(string to, string subject, string body, string attachmentName, byte[]? htmlContent);
    }
}
