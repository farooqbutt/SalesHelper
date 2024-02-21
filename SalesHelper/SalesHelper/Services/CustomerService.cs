using SalesHelper.Data;
using SalesHelper.Models;
using static SalesHelper.Services.ICustomerService;

namespace SalesHelper.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;
        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Create(Customer customer)
        {
            try
            {
                _context.Customers.Add(customer);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Delete(int id)
        {
            try
            {
                _context.Customers.Remove(Read(id));
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Customer Read(int id)
        {
            try
            {
                return _context.Customers.Find(id)!;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public List<Customer> ReadAll()
        {
            try
            {
                return _context.Customers.ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Update(Customer customer)
        {
            try
            {
                _context.Customers.Update(customer);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public NameType DetermineNameType(string name)
        {
            try
            {
                string[] nameParts = name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (nameParts.Length == 1)
                {
                    return NameType.First;
                }
                else if (nameParts.Length > 1)
                {
                    return NameType.Full;
                }
                else
                {
                    return NameType.Invalid;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
