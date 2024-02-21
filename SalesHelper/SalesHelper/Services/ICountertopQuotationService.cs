using SalesHelper.Models;

namespace SalesHelper.Services
{
    public interface ICountertopQuotationService
    {
        public void Create(CountertopQuotation address);
        public CountertopQuotation Read(int id);
        public List<CountertopQuotation> ReadAll();
        public void Update(CountertopQuotation address);
        public void Delete(int id);
    }
}
