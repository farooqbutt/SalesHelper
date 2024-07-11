using SalesHelper.Models;
using SalesHelper.Models.InterfaceModels;

namespace SalesHelper.Services
{
    public interface ISalesService
    {
        public void Create(CreateSaleInterface sale);
        public Sale Read(int id);
        public List<Sale> ReadAll(int accountNumber);
        public void Update(Sale sale);
        public void Delete(int id);
        public void UpdateStatus(int id, string status);
    }
}
