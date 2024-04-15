using SalesHelper.Models;
using SalesHelper.Models.InterfaceModels;

namespace SalesHelper.Services
{
    public interface ICountertopQuotationService
    {
        public void Create(CountertopQuotation address);
        public CountertopQuotation Read(int id);
        public List<CountertopQuotation> ReadAll();
        public void Update(CountertopQuotation address);
        public void Delete(int id);
        public List<string> GetBrandsForMaterial(string material);
        public List<CountertopBrandsData> GetColorsForBrand(string brand, string color);
        public Task<List<QuoteEmailInterface>> ReadCountertopQuoteEmails(int quoteId, ApplicationUser user); // Read Cabinet Quote Emails
    }
}
