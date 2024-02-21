using Microsoft.EntityFrameworkCore;
using SalesHelper.Data;
using SalesHelper.Models;

namespace SalesHelper.Services
{
    public class CountertopQuotationService : ICountertopQuotationService
    {
        private readonly ApplicationDbContext _context;
        public CountertopQuotationService(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Create(CountertopQuotation quotation)
        {
            try
            {
                _context.CountertopQuotations.Add(quotation);
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
                _context.CountertopQuotations.Remove(Read(id));
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public CountertopQuotation Read(int id)
        {
            try
            {
                return _context.CountertopQuotations.Find(id)!;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public List<CountertopQuotation> ReadAll()
        {
            try
            {
                return _context.CountertopQuotations.Include(a => a.CustomerIdFk).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Update(CountertopQuotation quotation)
        {
            try
            {
                _context.CountertopQuotations.Update(quotation);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
