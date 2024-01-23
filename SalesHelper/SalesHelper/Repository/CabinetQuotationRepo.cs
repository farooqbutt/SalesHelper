using SalesHelper.Data;
using SalesHelper.Models;

namespace SalesHelper.Repository
{
    public class CabinetQuotationRepo : ICabinetQuotationRepo
    {
        private readonly ApplicationDbContext _context;
        public CabinetQuotationRepo(ApplicationDbContext context)
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
                throw;
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
                throw;
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
                throw;
            }
        }

        public List<CabinetQuotation> ReadAll()
        {
            try
            {
                return _context.CabinetQuotations.ToList();
            }
            catch (Exception e)
            {
                throw;
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
                throw;
            }
        }
    }
}
