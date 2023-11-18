using SalesHelper.Data;
using SalesHelper.Models;

namespace SalesHelper.Repository
{
    public class CustomerRepo : ICustomerRepo
    {
        private readonly ApplicationDbContext _context;
        public CustomerRepo(ApplicationDbContext context)
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
                throw;
            }
        }

        public void Delete(int id)
        {
            try
            {
                _context.Customers.Remove(Read(id));
                _context.SaveChanges();
            }
            catch(Exception e)
            {
                throw;
            }
        }

        public Customer Read(int id)
        {
            try
            {
                return _context.Customers.Find(id)!;
            }
            catch(Exception e)
            {
                throw;
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
                throw;
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
                throw;
            }
        }
    }
}
