using SalesHelper.Models;

namespace SalesHelper.Repository
{
    public interface IAddressRepo
    {
        public void Create(Address address);
        public Address Read(int id);
        public List<Address> ReadAll();
        public void Update(Address address);
        public void Delete(int id);
    }
}
