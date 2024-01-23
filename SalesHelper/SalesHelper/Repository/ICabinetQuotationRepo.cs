using SalesHelper.Models;

namespace SalesHelper.Repository
{
    public interface ICabinetQuotationRepo
    {
        public void Create(CabinetQuotation cabinetQuotation);
        public CabinetQuotation Read(int id);
        public List<CabinetQuotation> ReadAll();
        public void Update(CabinetQuotation cabinetQuotation);
        public void Delete(int id);
    }
}
