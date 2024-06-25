using SalesHelper.Data;
using SalesHelper.Models;

namespace SalesHelper.Services
{
    public class SalesService : ISalesService
    {
        private readonly ApplicationDbContext _context;
        private readonly CabinetQuotationService _cabinetQuotationService;
        private readonly CountertopQuotationService _countertopQuotationService;
        public SalesService(ApplicationDbContext context, CabinetQuotationService cabinetQuotationService, CountertopQuotationService countertopQuotationService)
        {
            _context = context;
            _cabinetQuotationService = cabinetQuotationService;
            _countertopQuotationService = countertopQuotationService;
        }
        public void Create(Sale sale)
        {
            try
            {
                _context.Sales.Add(sale);
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
                _context.Sales.Remove(Read(id));
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Sale Read(int id)
        {
            try
            {
                return _context.Sales.Find(id)!;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public List<Sale> ReadAll(int accountNumber)
        {
            try
            {
                return _context.Sales.Where(a => a.CreatedByFK.AccountNumber == accountNumber).OrderByDescending(a => a.SalesDate).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Update(Sale sale)
        {
            try
            {
                _context.Sales.Update(sale);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Sale CreateSaleForView(int quoteId, string quoteType)
        {
            try
            {
                if (quoteType == "Cabinet")
                {
                    return new Sale
                    {
                        CabinetQuotationId = quoteId,
                        CabinetQuotationFK = _cabinetQuotationService.Read(quoteId),
                        Status = SaleStatus.Pending,
                        SalesDate = DateTime.Now
                    };
                }
                else if (quoteType == "Countertop")
                {
                    return new Sale
                    {
                        CountertopQuotationId = quoteId,
                        CountertopQuotationFK = _countertopQuotationService.Read(quoteId),
                        Status = SaleStatus.Pending,
                        SalesDate = DateTime.Now
                    };
                }
                else
                {
                    return new Sale();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
