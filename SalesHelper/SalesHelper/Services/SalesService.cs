using Microsoft.EntityFrameworkCore;
using SalesHelper.Data;
using SalesHelper.Models;
using SalesHelper.Models.InterfaceModels;

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
        public void Create(CreateSaleInterface sale)
        {
            try
            {
                _context.Sales.Add(sale.SaleData);
                _context.SaveChanges();
                //assign sales id to each cabinet item and save to database
                foreach (var item in sale.CabinetItems)
                {
                    item.SalesId = sale.SaleData.Id;
                    _context.SalesCabinetItems.Add(item);
                }
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
                return _context.Sales.Where(a => a.CreatedByFK.AccountNumber == accountNumber)
                    .Include(a => a.CreatedByFK)
                    .Include(a => a.CustomerFK)
                    .OrderByDescending(a => a.SalesDate).ToList();
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

        public void UpdateStatus(int id, string status)
        {
            try
            {
                var sale = Read(id);
                sale.Status = status;
                _context.Sales.Update(sale);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
