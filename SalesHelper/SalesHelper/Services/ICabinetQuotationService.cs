using MimeKit;
using SalesHelper.Models;
using SalesHelper.Models.InterfaceModels;

namespace SalesHelper.Services
{
    public interface ICabinetQuotationService
    {
        public void Create(CabinetQuotation cabinetQuotation);
        public CabinetQuotation Read(int id);
        public List<CabinetQuotation> ReadAll();
        public void Update(CabinetQuotation cabinetQuotation);
        public void Delete(int id);
        public CabinetQuoteInterface CabinetQuoteInterface(CabinetQuotation cabinetQuotation); // Cabinet Quotation Interface Creator
        public Task<List<QuoteEmailInterface>> ReadCabinetQuoteEmails(int quoteId, ApplicationUser user); // Read Cabinet Quote Emails
    }
}
