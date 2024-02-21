using SalesHelper.Models;

namespace SalesHelper.Services
{
    public interface ICustomerService
    {
        public void Create(Customer customer);
        public Customer Read(int id);
        public List<Customer> ReadAll();
        public void Update(Customer customer);
        public void Delete(int id);

        // Customer Name Checker for First Name or Full Name
        enum NameType
        {
            First,
            Full,
            Invalid
        }
        public NameType DetermineNameType(string name);
    }
}
