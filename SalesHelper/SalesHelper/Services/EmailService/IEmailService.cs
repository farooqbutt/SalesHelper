using SalesHelper.Models;
using SalesHelper.Models.InterfaceModels;

namespace SalesHelper.Services.EmailService
{
    public interface IEmailService
    {
        Task SendSimpleEmail(string to, string subject, string body, IFormFile attachment, QuotationEmails quoteEmail);
        Task SendEstimateRequestEmail(string to, string subject, string body, string attachmentName, byte[]? pdfBytes, IFormFile attachment, QuotationEmails quoteEmail);

        // Get Email Folders
        Task<QuoteEmailsInterface> GetQuoteInboxEmails(int quoteId);

        Task<QuoteEmailsInterface> GetQuoteSentEmails(int quoteId);

        Task<EmailMessage> GetInboxEmailMessageDetails(string messageId);

        Task<EmailMessage> GetSentEmailMessageDetails(string messageId);
    }
}
