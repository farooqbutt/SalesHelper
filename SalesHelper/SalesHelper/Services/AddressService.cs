using SalesHelper.Data;
using SalesHelper.Models;

namespace SalesHelper.Services
{
    public class AddressService : IAddressService
    {
        private readonly ApplicationDbContext _context;
        public AddressService(ApplicationDbContext context)
        {
            _context = context;
        }
        public Address Create(Address address)
        {
            try
            {
                _context.Address.Add(address);
                _context.SaveChanges();
                return address;
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
                _context.Address.Remove(Read(id));
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
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
                throw new Exception(e.Message);
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
                throw new Exception(e.Message);
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
                throw new Exception(e.Message);
            }
        }
    }
}
