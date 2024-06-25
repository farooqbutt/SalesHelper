using SalesHelper.Models;

namespace SalesHelper.Services
{
    public interface ISalesService
    {
        public void Create(Sale vendor);
        public Sale Read(int id);
        public List<Sale> ReadAll(int accountNumber);
        public void Update(Sale vendor);
        public void Delete(int id);

        public Sale CreateSaleForView(int quoteId, string quoteType);
    }
}
