using SalesHelper.Models;

namespace SalesHelper.Repository
{
    public interface ICountertopQuotationRepo
    {
        public void Create(CountertopQuotation address);
        public CountertopQuotation Read(int id);
        public List<CountertopQuotation> ReadAll();
        public void Update(CountertopQuotation address);
        public void Delete(int id);
    }
}
