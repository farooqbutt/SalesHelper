using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using SalesHelper.Data;
using SalesHelper.Models;

namespace SalesHelper.Services
{
    public class VendorService : IVendorService
    {
        private readonly ApplicationDbContext _context;
        public VendorService(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Create(Vendor vendor)
        {
            try
            {
                _context.Vendor.Add(vendor);
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
                _context.Vendor.Remove(Read(id));
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Vendor Read(int id)
        {
            try
            {
                return _context.Vendor.Include(v => v.AccountNumberIdFK)
                                      .Include(v => v.BusinessAddressIdFK)
                                      .Include(v => v.BillingAddressIdFK)
                                      .Include(v => v.ShippingAddressIdFK)
                                      .Include(v => v.BusinessTypeIdFK)
                                      .Where(v => v.VendorId == id).FirstOrDefault()!;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public List<Vendor> ReadAll()
        {
            try
            {
                return _context.Vendor.Include(v => v.AccountNumberIdFK)
                                      .Include(v => v.BusinessAddressIdFK)
                                      .Include(v => v.BillingAddressIdFK)
                                      .Include(v => v.ShippingAddressIdFK)
                                      .Include(v => v.BusinessTypeIdFK)
                                      .ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Update(Vendor vendor)
        {
            try
            {
                _context.Vendor.Update(vendor);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
