using Microsoft.EntityFrameworkCore;
using SalesHelper.Data;
using SalesHelper.Models;

namespace SalesHelper.Services
{
    public class CabinetQuotationService : ICabinetQuotationService
    {
        private readonly ApplicationDbContext _context;
        public CabinetQuotationService(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Create(CabinetQuotation cabinetQuotation)
        {
            try
            {
                _context.CabinetQuotations.Add(cabinetQuotation);
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
                _context.CabinetQuotations.Remove(Read(id));
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public CabinetQuotation Read(int id)
        {
            try
            {
                return _context.CabinetQuotations.Find(id)!;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public List<CabinetQuotation> ReadAll()
        {
            try
            {
                return _context.CabinetQuotations.Include(a => a.VendorIdFk).Include(a => a.CustomerIdFk).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Update(CabinetQuotation cabinetQuotation)
        {
            try
            {
                _context.CabinetQuotations.Update(cabinetQuotation);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
