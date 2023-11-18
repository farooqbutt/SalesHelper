using SalesHelper.Models;

namespace SalesHelper.Repository
{
    public interface ICustomerRepo
    {
        public void Create(Customer customer);
        public Customer Read(int id);
        public List<Customer> ReadAll();
        public void Update(Customer customer);
        public void Delete(int id);
    }
}
