using SalesHelper.Models;

namespace SalesHelper.Services.EmailService
{
    public interface IEmailService
    {
        Task<bool> SendAccountConfirmationEmail(string to, string subject, string htmlMessage);
        Task SendEstimateRequestEmail(int accountNumber, string to, string subject, string body, string attachmentName, byte[]? pdfBytes, IFormFile attachment, QuotationEmails quoteEmail);
    }
}
