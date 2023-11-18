using SalesHelper.Data;
using SalesHelper.Models;

namespace SalesHelper.Repository
{
    public class AddressRepo : IAddressRepo
    {
        private readonly ApplicationDbContext _context;
        public AddressRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Create(Address address)
        {
            try
            {
                _context.Address.Add(address);
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
                _context.Address.Remove(Read(id));
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public Address Read(int id)
        {
            try
            {
                return _context.Address.Find(id)!;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public List<Address> ReadAll()
        {
            try
            {
                return _context.Address.ToList();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void Update(Address address)
        {
            try
            {
                _context.Address.Update(address);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
