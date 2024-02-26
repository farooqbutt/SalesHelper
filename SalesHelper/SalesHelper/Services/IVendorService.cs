using SalesHelper.Models;

namespace SalesHelper.Services
{
    public interface IVendorService
    {
        public void Create(Vendor vendor);
        public Vendor Read(int id);
        public List<Vendor> ReadAll();
        public void Update(Vendor vendor);
        public void Delete(int id);
    }
}
