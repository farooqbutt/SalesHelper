using SalesHelper.Models;

namespace SalesHelper.Services
{
    public interface IAddressService
    {
        public Address Create(Address address);
        public Address Read(int id);
        public List<Address> ReadAll();
        public void Update(Address address);
        public void Delete(int id);
    }
}
